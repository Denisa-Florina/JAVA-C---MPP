
namespace TransportModel.transport.model;

[Serializable]
public class Flight : Entity<int>
{
    public string Destination { get; set; }
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public string Airport { get; set; }
    public int AvailableSeats { get; set; }
    
    public Flight(string destination, DateTime departureTime, DateTime arrivalTime, string airport, int availableSeats)
    {
        Destination = destination;
        DepartureTime = departureTime;
        ArrivalTime = arrivalTime;
        Airport = airport;
        AvailableSeats = availableSeats;
    }

    public override string ToString()
    {
        return Destination + " " + DepartureTime + " " + ArrivalTime;
    }
}