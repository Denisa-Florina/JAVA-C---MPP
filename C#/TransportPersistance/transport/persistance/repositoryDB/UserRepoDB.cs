using System.Data;
using System.Data.SQLite;
using log4net;
using log4net.Repository.Hierarchy;

using tasks.repository;
using TransportModel.transport.model;

namespace proiect.repository.repositoryDB;

public class UserRepoDB
{
    private static readonly ILog logger = LogManager.GetLogger("UserRepoDB");
    private readonly IDictionary<String, string> connectionString;

    public UserRepoDB(IDictionary<String, string> connectionString)
    {
        logger.Info("Initializing UserRepoBD");
        this.connectionString = connectionString;
    }

    public void Add(User user)
    {
        logger.InfoFormat("Saving user: {0}", user);
    
        logger.Info("Connecting to database...");
        logger.Info($"Using connection string: {connectionString["ConnectionString"]}");
        var con = DBUtils.getConnection(connectionString);
    
        using (var comm = con.CreateCommand())
        {
            logger.Info("Inserting user...");
            comm.CommandText = "INSERT INTO users (username, password) VALUES (@username, @password)";
            logger.Info($"SQL Command: {comm.CommandText}");
            
            var paramUsername = comm.CreateParameter();
            paramUsername.ParameterName = "@username";
            paramUsername.Value = user.Username;
            comm.Parameters.Add(paramUsername);

            var paramPassword = comm.CreateParameter();
            paramPassword.ParameterName = "@password";
            paramPassword.Value = user.Password;
            comm.Parameters.Add(paramPassword);

            var result = comm.ExecuteNonQuery();
        
            if (result == 0)
            {
                throw new Exception("User not found");
            }
            else
            {
                logger.InfoFormat("User saved: {0}", user);
            }
        }
    }
    
    public List<User> FindAll()
    {
        logger.Info("Retrieving all users...");

        var users = new List<User>();
        var con = DBUtils.getConnection(connectionString);

        using (var comm = con.CreateCommand())
        {
            logger.Info("Executing SELECT command to fetch all users...");
            comm.CommandText = "SELECT username, password FROM users";
            logger.Info($"SQL Command: {comm.CommandText}");

            using (var reader = comm.ExecuteReader())
            {
                while (reader.Read())
                {
                    var user = new User(reader["username"].ToString(), reader["password"].ToString());
                    users.Add(user);
                }
            }
        }

        logger.Info($"Found {users.Count} users.");
        return users;
    }
    
    public User Find(string username)
    {
        logger.Info($"Searching for user with username: {username}");

        var con = DBUtils.getConnection(connectionString);
        using (var comm = con.CreateCommand())
        {
            logger.Info("Executing SELECT command to fetch user by username...");
            comm.CommandText = "SELECT username, password FROM users WHERE username = @username";
            logger.Info($"SQL Command: {comm.CommandText}");

            var paramUsername = comm.CreateParameter();
            paramUsername.ParameterName = "@username";
            paramUsername.Value = username;
            comm.Parameters.Add(paramUsername);

            using (var reader = comm.ExecuteReader())
            {
                if (reader.Read())  // If we find a match
                {
                    var user = new User(reader["username"].ToString(), reader["password"].ToString());
                    logger.Info($"User found: {user}");
                    return user;
                }
                else
                {
                    logger.Info("User not found.");
                    return null;  // No user found
                }
            }
        }
    }

}

