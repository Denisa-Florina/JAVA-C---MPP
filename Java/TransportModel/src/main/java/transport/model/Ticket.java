package transport.model;

import java.io.Serializable;
import java.util.List;

public class Ticket extends Entity<Integer> implements Serializable {
    private List<String> passager;
    private int seatCount;
    private int flightId;

    public Ticket(List<String> passager, int seatCount, int flightId) {
        this.passager = passager;
        this.seatCount = seatCount;
        this.flightId = flightId;
    }

    public int getFlightId() {
        return flightId;
    }

    public void setFlightId(int flightId) {
        this.flightId = flightId;
    }

    public List<String> getPassager() {
        return passager;
    }

    public void setPassager(List<String> passager) {
        this.passager = passager;
    }

    public int getSeatCount() {
        return seatCount;
    }

    public void setSeatCount(int seatCount) {
        this.seatCount = seatCount;
    }

    @Override
    public String toString() {
        return "transport.model.Ticket{" +
                "passager=" + passager +
                ", seatCount=" + seatCount +
                '}';
    }
}
