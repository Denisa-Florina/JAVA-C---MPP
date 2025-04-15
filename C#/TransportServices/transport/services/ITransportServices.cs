using TransportModel.transport.model;

namespace TransportServices.transport.services;

public interface ITransportServices {
    bool Login(string username, string password, ITransportObserver client);
    bool BuyTicket(string destination, int requiredSeats, int seats, List<string> passengers);
    List<Flight> SearchFlights(string destination, DateTime departure);
    List<Flight> GetAllFlights();
}