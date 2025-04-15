package transport.persistence.repositoryBD;

import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import transport.model.Ticket;
import transport.persistence.TicketRepository;
import transport.persistence.databse.JdbcUtils;

import java.sql.*;
import java.util.ArrayList;
import java.util.List;
import java.util.Properties;

public class TicketRepoBD implements TicketRepository {
    private JdbcUtils dbUtils;
    private static final Logger logger = LogManager.getLogger();

    public TicketRepoBD(Properties prop) {
        logger.info("Initializing TicketRepoBD with properties: {} ");
        dbUtils = new JdbcUtils(prop);
    }

    @Override
    public void add(Ticket ticket) {
        logger.traceEntry("saving task {}", ticket);
        String sql = "INSERT INTO Ticket (passager, seat_count, idFlight) VALUES (?, ?, ?)";
        Connection connection = dbUtils.getConnection();
        try (PreparedStatement stmt = connection.prepareStatement(sql)) {
            String passagerString = String.join(",", ticket.getPassager());
            stmt.setString(1, passagerString);
            stmt.setInt(2, ticket.getSeatCount());
            stmt.setInt(3, ticket.getFlightId());
            stmt.executeUpdate();
        } catch (SQLException e) {
            e.printStackTrace();
        }
    }

    @Override
    public void update(Integer id, Ticket ticket) {
        logger.traceEntry("update ticket {}", ticket);
        String sql = "UPDATE Ticket SET passager=?, seat_count=?, idFlight=? WHERE id=?";
        Connection connection = dbUtils.getConnection();
        try (PreparedStatement stmt = connection.prepareStatement(sql)) {
            String passagerString = String.join(",", ticket.getPassager());
            stmt.setString(1, passagerString);
            stmt.setInt(2, ticket.getSeatCount());
            stmt.setInt(3, ticket.getFlightId());
            stmt.setInt(4, id);
            stmt.executeUpdate();
        } catch (SQLException e) {
            logger.error("Error updating ticket", e);
        }
    }

    @Override
    public void delete(Integer id) {
        logger.traceEntry("delete task {}", id);
        String sql = "DELETE FROM Ticket WHERE id=?";
        Connection connection = dbUtils.getConnection();
        try (PreparedStatement stmt = connection.prepareStatement(sql)) {
            stmt.setInt(1, id);
            stmt.executeUpdate();
        } catch (SQLException e) {
            e.printStackTrace();
        }
    }

    @Override
    public Ticket find(Integer id) {
        logger.traceEntry("find task {}", id);
        String sql = "SELECT * FROM Ticket WHERE id=?";
        Connection connection = dbUtils.getConnection();
        try (PreparedStatement stmt = connection.prepareStatement(sql)) {
            stmt.setInt(1, id);
            ResultSet rs = stmt.executeQuery();
            if (rs.next()) {
                String passagerString = rs.getString("passager");
                List<String> passager = List.of(passagerString.split(","));
                int idFlight = rs.getInt("idFlight");
                Ticket ticket = new Ticket(passager ,rs.getInt("seat_count"), idFlight);
                ticket.setId(rs.getInt("id"));
                return ticket;
            }
        } catch (SQLException e) {
            e.printStackTrace();
        }
        return null;
    }

    @Override
    public Iterable<Ticket> findAll() {
        logger.traceEntry("find all tickets");
        List<Ticket> tickets = new ArrayList<>();
        String sql = "SELECT * FROM Ticket";
        Connection connection = dbUtils.getConnection();
        try (Statement stmt = connection.createStatement(); ResultSet rs = stmt.executeQuery(sql)) {
            while (rs.next()) {
                String passagerString = rs.getString("passager");
                List<String> passager = List.of(passagerString.split(","));
                int flightId = rs.getInt("idFlight");
                Ticket ticket = new Ticket(passager, rs.getInt("seat_count"), flightId);
                ticket.setId(rs.getInt("id"));
                tickets.add(ticket);
            }
        } catch (SQLException e) {
            logger.error("Error retrieving all tickets", e);
        }
        return tickets;
    }

}
