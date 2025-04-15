namespace TransportModel.transport.model;

[Serializable]
public class Ticket : Entity<int>
{
    public List<string> Passager { get; set; }
    public int SeatCount { get; set; }
    public int FlightId { get; set; }
    
    public Ticket(int seatCount, List<string> passager, int flightId)
    {
        SeatCount = seatCount;
        Passager = passager;
        FlightId = flightId;
    }

    public override string ToString()
    {
        return "Ticket: " + Id + " Nr locuri: " +  SeatCount;
    }
}