using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using log4net;

using proiect.repository.repositoryDB;
using Transport.Services;
using TransportModel.transport.model;
using TransportServices.transport.services;

public class TransportServicesImpl : ITransportServices
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(TransportServicesImpl));
        private readonly FlightRepoDB flightRepository;
        private readonly TicketRepoDB ticketRepository;
        private readonly UserRepoDB userRepository;
        private User loginUser;
        private readonly IDictionary<string, ITransportObserver> loggedClients;

        public TransportServicesImpl(FlightRepoDB flightRepository, TicketRepoDB ticketRepository, UserRepoDB userRepository)
        {
            this.flightRepository = flightRepository;
            this.ticketRepository = ticketRepository;
            this.userRepository = userRepository;
            this.loggedClients = new Dictionary<string, ITransportObserver>();
        }


        public void AddFlight(Flight flight)
        {
            logger.Info($"Adding flight: {flight}");
            flightRepository.Add(flight);
        }


        public Flight FindFlight(int id)
        {
            lock (this)
            {
                return flightRepository.Find(id);
            }
        }


        public List<Flight> GetAllFlights()
        {
            lock (this)
            {
                return flightRepository.FindAll().ToList();
            }
        }


        public void UpdateFlight(int id, Flight flight)
        {
            lock (this)
            {
                flightRepository.Update(id, flight);
            }
        }


        public void DeleteFlight(int id)
        {
            lock (this)
            {
                flightRepository.Delete(id);
            }
        }


        public List<Flight> SearchFlights(string destination, DateTime departureDate)
        {
            lock (this)
            {
                return flightRepository.SearchByDestinationAndDate(destination, departureDate);
            }
        }


        public bool BuyTicket(string destination, int requiredseats, int seats, List<string> passengers)
        {
            lock (this)
            {
                var flight = flightRepository.SearchBySeatsAndDestination(destination, requiredseats).FirstOrDefault();

                Console.WriteLine("flight: " + flight);

                if (flight == null || flight.AvailableSeats < seats)
                {
                    Console.WriteLine("nu o gasit repo-ul zborul");
                    return false;
                }

                var ticket = new Ticket(seats, passengers, flight.Id);
                ticketRepository.Add(ticket);
                UpdateFlightSeats(flight.Id, -seats);
                
                
                foreach (ITransportObserver client in loggedClients.Values)
                {
                    try
                    {
                        Task.Run(() => client.ReservationAdded("da"));
                    }
                    catch (TransportException e)
                    {
                        Console.Error.WriteLine("Error notifying client in TransportServicesImpl: " + e);
                    }
                }
                
                
                return true;
            }
        }


        public Ticket FindTicket(int id)
        {
            lock (this)
            {
                return ticketRepository.Find(id);
            }
        }


        public IEnumerable<Ticket> GetAllTickets()
        {
            lock (this)
            {
                return ticketRepository.FindAll();
            }
        }
        

 
        private void UpdateFlightSeats(int flightId, int seatChange)
        {
            lock (this)
            {
                var flight = flightRepository.Find(flightId);
                if (flight != null)
                {
                    int newSeats = flight.AvailableSeats + seatChange;
                    if (newSeats < 0)
                    {
                        logger.Warn($"Attempted to reduce seats below zero for flight {flightId}");
                        return;
                    }

                    flight.AvailableSeats = newSeats;
                    flightRepository.Update(flightId, flight);
                }
            }
        }
        
        public void AddUser(User user)
        {
            lock (this)
            {
                logger.Info($"Adding user: {user}");
                string hashedPassword = HashPassword(user.Password);
                user.Password = hashedPassword;
                userRepository.Add(user);
            }
        }


        public User FindUser(string username)
        {
            lock (this)
            {
                return userRepository.Find(username);
            }
        }


        public IEnumerable<User> GetAllUsers()
        {
            lock (this)
            {
                return userRepository.FindAll();
            }
        }


        public bool Login(string username, string password, ITransportObserver observer)
        {
            lock (this)
            {
                Console.WriteLine("In serverImp: try login");
                var user = userRepository.Find(username);
                Console.WriteLine(username);
                if (user != null && CheckPassword(password, user.Password))
                {
                    Console.WriteLine("In serverImp: Login successful");
                    loggedClients[user.Username] = observer;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        

        public string HashPassword(string password)
        {
            lock (this)
            {
                return BCrypt.Net.BCrypt.HashPassword(password);
            }
        }


        private bool CheckPassword(string password, string hashedPassword)
        {
            lock (this)
            {
                return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
            }
        }
    }
