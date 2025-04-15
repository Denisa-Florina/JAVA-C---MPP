package transport.persistence.repositoryBD;

import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import transport.model.Flight;
import transport.persistence.FlightRepository;
import transport.persistence.databse.JdbcUtils;


import java.sql.*;
import java.time.LocalDate;
import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.List;
import java.util.Properties;

public class FlightRepoBD implements FlightRepository {
    private JdbcUtils dbUtils;
    private static final Logger logger= LogManager.getLogger();

    public FlightRepoBD(Properties prop) {
        logger.info("Initializing FlightRepoBD with properties: {} ");
        dbUtils = new JdbcUtils(prop);
    }

    @Override
    public void add(Flight flight) {
        logger.traceEntry("saving task {}", flight);
        Connection connection = dbUtils.getConnection();
        String sql = "INSERT INTO Flight (destination, departure_time, arrival_time, airport, available_seats) VALUES (?, ?, ?, ?, ?)";
        try (PreparedStatement stmt = connection.prepareStatement(sql)) {
            stmt.setString(1, flight.getDestination());
            stmt.setTimestamp(2, Timestamp.valueOf(flight.getDepartureTime()));
            stmt.setTimestamp(3, Timestamp.valueOf(flight.getArrivalTime()));
            stmt.setString(4, flight.getAirport());
            stmt.setInt(5, flight.getAvailableSeats());
            stmt.executeUpdate();
        } catch (SQLException e) {
            e.printStackTrace();
        }
    }

    @Override
    public void update(Integer id, Flight flight) {
        logger.traceEntry("update task {}", flight);
        String sql = "UPDATE Flight SET destination=?, departure_time=?, arrival_time=?, airport=?, available_seats=? WHERE id=?";
        Connection connection = dbUtils.getConnection();
        try (PreparedStatement stmt = connection.prepareStatement(sql)) {
            stmt.setString(1, flight.getDestination());
            stmt.setTimestamp(2, Timestamp.valueOf(flight.getDepartureTime()));
            stmt.setTimestamp(3, Timestamp.valueOf(flight.getArrivalTime()));
            stmt.setString(4, flight.getAirport());
            stmt.setInt(5, flight.getAvailableSeats());
            stmt.setInt(6, id);
            stmt.executeUpdate();
        } catch (SQLException e) {
            e.printStackTrace();
        }
    }

    @Override
    public void delete(Integer id) {
        logger.traceEntry("delete task {}", id);
        String sql = "DELETE FROM Flight WHERE id=?";
        Connection connection = dbUtils.getConnection();
        try (PreparedStatement stmt = connection.prepareStatement(sql)) {
            stmt.setInt(1, id);
            stmt.executeUpdate();
        } catch (SQLException e) {
            e.printStackTrace();
        }
    }

    @Override
    public Flight find(Integer id) {
        logger.traceEntry("find task {}", id);
        String sql = "SELECT * FROM Flight WHERE id=?";
        Connection connection = dbUtils.getConnection();
        try (PreparedStatement stmt = connection.prepareStatement(sql)) {
            stmt.setInt(1, id);
            ResultSet rs = stmt.executeQuery();
            if (rs.next()) {
                Flight flight = new Flight(
                        rs.getString("destination"),
                        rs.getTimestamp("departure_time").toLocalDateTime(),
                        rs.getTimestamp("arrival_time").toLocalDateTime(),
                        rs.getString("airport"),
                        rs.getInt("available_seats")
                );
                flight.setId(rs.getInt("id"));
                return flight;
            }
        } catch (SQLException e) {
            e.printStackTrace();
        }
        return null;
    }

    @Override
    public Iterable<Flight> findAll() {
        logger.traceEntry("find all task {}");
        List<Flight> flights = new ArrayList<>();
        String sql = "SELECT * FROM Flight";
        Connection connection = dbUtils.getConnection();
        try (Statement stmt = connection.createStatement(); ResultSet rs = stmt.executeQuery(sql)) {
            while (rs.next()) {
                Flight flight = new Flight(
                        rs.getString("destination"),
                        rs.getTimestamp("departure_time").toLocalDateTime(),
                        rs.getTimestamp("arrival_time").toLocalDateTime(),
                        rs.getString("airport"),
                        rs.getInt("available_seats")
                );
                flight.setId(rs.getInt("id"));
                flights.add(flight);

            }
        } catch (SQLException e) {
            e.printStackTrace();
        }
        return flights;
    }

    public List<Flight> searchByDestinationAndDate(String destination, LocalDate departureDate) {
        List<Flight> result = new ArrayList<>();
        String sql = "SELECT * FROM Flight WHERE LOWER(destination) = LOWER(?) AND departure_time >= ? AND departure_time < ?";
        try (Connection connection = dbUtils.getConnection();
             PreparedStatement stmt = connection.prepareStatement(sql)) {
            LocalDateTime start = departureDate.atStartOfDay();
            LocalDateTime end = start.plusDays(1);

            stmt.setString(1, destination);
            stmt.setTimestamp(2, Timestamp.valueOf(start));
            stmt.setTimestamp(3, Timestamp.valueOf(end));
            try (ResultSet rs = stmt.executeQuery()) {
                while (rs.next()) {
                    int id = rs.getInt("id");
                    String dest = rs.getString("destination");
                    LocalDateTime departureTime = rs.getTimestamp("departure_time").toLocalDateTime();
                    LocalDateTime arrivalTime = rs.getTimestamp("arrival_time").toLocalDateTime();
                    String airport = rs.getString("airport");
                    int availableSeats = rs.getInt("available_seats");
                    Flight flight = new Flight( dest, departureTime, arrivalTime, airport, availableSeats);
                    flight.setId(id);
                    result.add(flight);
                }
            }
        } catch (SQLException e) {
            logger.error("Database error in searchByDestinationAndDate: ", e);
        }
        return result;
    }

    public List<Flight> searchByDestinationAndDateTime(String destination, LocalDateTime departureDateTime) {
        List<Flight> result = new ArrayList<>();
        String sql = "SELECT * FROM Flight WHERE LOWER(destination) = LOWER(?) AND departure_time = ?";
        try (Connection connection = dbUtils.getConnection();
             PreparedStatement stmt = connection.prepareStatement(sql)) {

            stmt.setString(1, destination);
            stmt.setTimestamp(2, Timestamp.valueOf(departureDateTime));

            try (ResultSet rs = stmt.executeQuery()) {
                while (rs.next()) {
                    int id = rs.getInt("id");
                    String dest = rs.getString("destination");
                    LocalDateTime departureTime = rs.getTimestamp("departure_time").toLocalDateTime();
                    LocalDateTime arrivalTime = rs.getTimestamp("arrival_time").toLocalDateTime();
                    String airport = rs.getString("airport");
                    int availableSeats = rs.getInt("available_seats");
                    Flight flight = new Flight(dest, departureTime, arrivalTime, airport, availableSeats);
                    flight.setId(id);
                    result.add(flight);
                }
            }
        } catch (SQLException e) {
            logger.error("Database error in searchByDestinationAndDateTime: ", e);
        }
        return result;
    }

}
