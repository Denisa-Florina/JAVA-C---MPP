package transport.persistence.repositoryBD;

import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;
import transport.model.User;
import transport.persistence.databse.JdbcUtils;


import java.sql.*;
import java.util.ArrayList;
import java.util.List;
import java.util.Properties;

public class UserRepoBD {
    private JdbcUtils dbUtils;
    private static final Logger logger= LogManager.getLogger();

    public UserRepoBD(Properties prop) {
        logger.info("Initializing UserRepoBD with properties: {} ");
        dbUtils = new JdbcUtils(prop);
    }

    public void add(User user) {
        logger.info("saved user: {} ");
        Connection connection = null;
        String sql = "INSERT INTO users (username, password) VALUES (?, ?)";
        try {
            connection = dbUtils.getConnection();  // Get the connection
            try (PreparedStatement stmt = connection.prepareStatement(sql)) {
                stmt.setString(1, user.getUsername());
                stmt.setString(2, user.getPassword());
                stmt.executeUpdate();
                logger.info("transport.model.User saved successfully: {}", user.getUsername());  // Log success
            } catch (SQLException e) {
                logger.error("Error saving user: {}", user.getUsername(), e);  // Log error
            }
        } finally {
            if (connection != null) {
                try {
                    connection.close();  // Ensure the connection is closed
                } catch (SQLException e) {
                    logger.error("Error closing database connection", e);
                }
            }
        }
    }


    public User find(String username) {
        logger.info("Finding user: {} ");
        String sql = "SELECT * FROM users WHERE username=?";
        Connection connection = dbUtils.getConnection();
        try (PreparedStatement stmt = connection.prepareStatement(sql)) {
            stmt.setString(1, username);
            ResultSet rs = stmt.executeQuery();
            if (rs.next()) {
                return new User(
                        rs.getString("username"),
                        rs.getString("password")
                );
            }
        } catch (SQLException e) {
            e.printStackTrace();
        }
        return null;
    }

    public Iterable<User> findAll() {
        logger.info("Finding all users : {} ");
        List<User> users = new ArrayList<>();
        String sql = "SELECT * FROM users";
        Connection connection = dbUtils.getConnection();
        try (Statement stmt = connection.createStatement(); ResultSet rs = stmt.executeQuery(sql)) {
            while (rs.next()) {
                users.add(new User(
                        rs.getString("username"),
                        rs.getString("password")
                ));
            }
        } catch (SQLException e) {
            e.printStackTrace();
        }
        return users;
    }
}
