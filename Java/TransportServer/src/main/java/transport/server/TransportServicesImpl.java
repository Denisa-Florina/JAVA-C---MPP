package transport.server;

import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;
import org.mindrot.jbcrypt.BCrypt;
import transport.model.Flight;
import transport.model.Ticket;
import transport.model.User;
import transport.persistence.repositoryBD.FlightRepoBD;
import transport.persistence.repositoryBD.TicketRepoBD;
import transport.persistence.repositoryBD.UserRepoBD;
import transport.services.ITransportObserver;
import transport.services.ITransportServices;
import transport.services.TransportException;


import java.time.LocalDate;
import java.time.LocalDateTime;
import java.time.format.DateTimeFormatter;
import java.util.*;
import java.util.concurrent.ConcurrentHashMap;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.stream.Collectors;

public class TransportServicesImpl implements ITransportServices {
    private static final Logger logger = LogManager.getLogger();
    private FlightRepoBD flightRepository;
    private TicketRepoBD ticketRepository;
    private UserRepoBD userRepository;

    private Map<String, ITransportObserver> loggedClient;
    private final Map<ITransportObserver, Boolean> observers = new ConcurrentHashMap<>();

    static private TransportServicesImpl instance;

    public static TransportServicesImpl getInstance(FlightRepoBD flightRepository, TicketRepoBD ticketRepository, UserRepoBD userRepository) {
        if (instance == null) {
            instance = new TransportServicesImpl(flightRepository, ticketRepository, userRepository);
        }
        return instance;
    }

    private TransportServicesImpl(FlightRepoBD flightRepository, TicketRepoBD ticketRepository, UserRepoBD userRepository) {
        this.flightRepository = flightRepository;
        this.ticketRepository = ticketRepository;
        this.userRepository = userRepository;
        loggedClient = new HashMap<>();
    }

    public List<String> searchFlights(String destination, LocalDate departureDate) throws TransportException {
        try{
            return flightRepository.searchByDestinationAndDate(destination, departureDate).stream()
                    .map(flight -> flight.getDestination() + " "+ flight.getDepartureTime().toLocalDate() + " " + flight.getDepartureTime().toLocalTime() +
                            " Seats: " + flight.getAvailableSeats())
                    .collect(Collectors.toList());
        }
        catch (Exception e){
            throw new TransportException(e.getMessage());
        }

    }

    @Override
    public List<Flight> getAllFlights() throws TransportException {
        try {
            System.out.println("Getting all flights... ");
            Iterable<Flight> iterableFlights = flightRepository.findAll();
            List<Flight> flightList = new ArrayList<>();
            iterableFlights.forEach(flightList::add);
            return flightList;
        }catch (Exception e){
            throw new TransportException("Ceva in repo nu a mers bine/n/n/n/n!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        }
    }

    public synchronized boolean buyTicket(String selectedFlight, int seats, List<String> passengers) {

        System.out.println("BuyTicket Service!!!");
        String destination = selectedFlight.split(" ")[0];
        String departure = selectedFlight.split(" ")[1] + " " + selectedFlight.split(" ")[2];;

        DateTimeFormatter formatter = DateTimeFormatter.ofPattern("yyyy-MM-dd HH:mm:ss");
        LocalDateTime departureDateTime = LocalDateTime.parse(departure, formatter);

        System.out.println(destination);
        System.out.println(departureDateTime);

        Flight flight = flightRepository.searchByDestinationAndDateTime(destination, departureDateTime).stream().findFirst().orElse(null);


        if (flight == null || flight.getAvailableSeats() < seats) {
            return false;
        }

        Ticket ticket = new Ticket(passengers, flight.getId(), seats);
        ticketRepository.add(ticket);
        updateFlightSeats(flight.getId(), -seats);


        ExecutorService executor = Executors.newSingleThreadExecutor();
        executor.submit(() -> {
            for (ITransportObserver observer : observers.keySet()) {
                try {
                    observer.reservationAdded("NOTIFICARE");
                    System.out.println("Notificare trimisă către observer: " + observer);
                } catch (TransportException e) {
                    System.err.println("Eroare la notificare: " + e.getMessage());
                }
            }
        });
        executor.shutdown();



        return true;
    }

    private synchronized void updateFlightSeats(Integer flightId, int seatChange) {
        Flight flight = flightRepository.find(flightId);
        if (flight != null) {
            int newSeats = flight.getAvailableSeats() + seatChange;
            if (newSeats < 0) {
                logger.warn("Attempted to reduce seats below zero for flight {}", flightId);
                return;
            }
            flight.setAvailableSeats(newSeats);
            flightRepository.update(flightId, flight);
        }
    }

    public synchronized void addUser(User user) {
        logger.info("Adding user: {}", user);

        String hashedPassword = hashPassword(user.getPassword());
        user.setPassword(hashedPassword);

        userRepository.add(user);
    }

    public User findUser(String username) {
        return userRepository.find(username);
    }


    public synchronized void login(String username, String password, ITransportObserver client) throws TransportException {
        User user = userRepository.find(username);
        if (user != null && checkPassword(password, user.getPassword())) {
            loggedClient.put(username, client);
            addObserver(client);
        } else {
            throw new TransportException("Invalid username or password");
        }
    }

    @Override
    public void logout(String username, ITransportObserver client) throws TransportException {
        ITransportObserver localClient = loggedClient.remove(username);
        removeObserver(localClient);
    }

    public synchronized String hashPassword(String password) {
        return BCrypt.hashpw(password, BCrypt.gensalt());
    }

    private synchronized boolean checkPassword(String password, String hashedPassword) {
        return BCrypt.checkpw(password, hashedPassword);
    }


    public void addObserver(ITransportObserver observer) {
        observers.put(observer, true);
    }

    public void removeObserver(ITransportObserver observer) {
        observers.remove(observer);
    }


}