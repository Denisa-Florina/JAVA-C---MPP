package transport.model;

import java.io.Serializable;
import java.time.LocalDateTime;

public class Flight extends Entity<Integer> implements Serializable {
    private String destination;
    private LocalDateTime departureTime;
    private LocalDateTime arrivalTime;
    private String airport;
    private int availableSeats;


    public Flight(String destination, LocalDateTime departureTime, LocalDateTime arrivalTime, String airport, int availableSeats) {
        this.destination = destination;
        this.departureTime = departureTime;
        this.arrivalTime = arrivalTime;
        this.airport = airport;
        this.availableSeats = availableSeats;
    }

    public String getDestination() {
        return destination;
    }

    public void setDestination(String destination) {
        this.destination = destination;
    }

    public LocalDateTime getDepartureTime() {
        return departureTime;
    }

    public void setDepartureTime(LocalDateTime departureTime) {
        this.departureTime = departureTime;
    }

    public LocalDateTime getArrivalTime() {
        return arrivalTime;
    }

    public void setArrivalTime(LocalDateTime arrivalTime) {
        this.arrivalTime = arrivalTime;
    }

    public String getAirport() {
        return airport;
    }

    public void setAirport(String airport) {
        this.airport = airport;
    }

    public int getAvailableSeats() {
        return availableSeats;
    }

    public void setAvailableSeats(int availableSeats) {
        this.availableSeats = availableSeats;
    }

    @Override
    public String toString() {
        return "Flight{" +
                "destination='" + destination + '\'' +
                ", departureTime=" + departureTime +
                ", arrivalTime=" + arrivalTime +
                ", airport='" + airport + '\'' +
                ", availableSeats=" + availableSeats +
                '}';
    }
}
