package transport.services;

import transport.model.Flight;
import transport.model.User;

import java.time.LocalDate;
import java.util.List;

public interface ITransportServices {
    void login(String username, String password, ITransportObserver client) throws TransportException;
    void logout(String username, ITransportObserver client) throws TransportException;
    String hashPassword(String password);
    User findUser(String username);
    void addUser(User user);
    boolean buyTicket(String Flight, int seats, List<String> passagers);
    List<String> searchFlights(String destination, LocalDate departure) throws TransportException ;
    List<Flight> getAllFlights() throws TransportException ;

}
