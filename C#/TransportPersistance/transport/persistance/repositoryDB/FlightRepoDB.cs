using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using log4net;

using proiect.repository;
using tasks.repository;
using transport.persistance;
using TransportModel.transport.model;


public class FlightRepoDB : IFlightRepository
{
    private static readonly ILog logger = LogManager.GetLogger("FlightRepoDB");
    private readonly IDictionary<string, string> connectionString;

    public FlightRepoDB(IDictionary<string, string> connectionString)
    {
        logger.Info("Initializing FlightRepoDB");
        this.connectionString = connectionString;
    }

    public void Add(Flight flight)
    {
        logger.InfoFormat("Saving flight: {0}", flight);

        logger.Info("Connecting to database...");
        logger.Info($"Using connection string: {connectionString["ConnectionString"]}");
        var con = DBUtils.getConnection(connectionString);

        using (var comm = con.CreateCommand())
        {
            logger.Info("Inserting flight...");
            comm.CommandText =
                "INSERT INTO Flight (destination, departure_time, arrival_time, airport, available_seats) VALUES (@destination, @departureTime, @arrivalTime, @airport, @availableSeats)";
            logger.Info($"SQL Command: {comm.CommandText}");

            var paramDestination = comm.CreateParameter();
            paramDestination.ParameterName = "@destination";
            paramDestination.Value = flight.Destination;
            comm.Parameters.Add(paramDestination);

            var paramDepartureTime = comm.CreateParameter();
            paramDepartureTime.ParameterName = "@departureTime";
            paramDepartureTime.Value = flight.DepartureTime;
            comm.Parameters.Add(paramDepartureTime);

            var paramArrivalTime = comm.CreateParameter();
            paramArrivalTime.ParameterName = "@arrivalTime";
            paramArrivalTime.Value = flight.ArrivalTime;
            comm.Parameters.Add(paramArrivalTime);

            var paramAirport = comm.CreateParameter();
            paramAirport.ParameterName = "@airport";
            paramAirport.Value = flight.Airport;
            comm.Parameters.Add(paramAirport);

            var paramAvailableSeats = comm.CreateParameter();
            paramAvailableSeats.ParameterName = "@availableSeats";
            paramAvailableSeats.Value = flight.AvailableSeats;
            comm.Parameters.Add(paramAvailableSeats);

            var result = comm.ExecuteNonQuery();

            if (result == 0)
            {
                throw new Exception("Flight not saved");
            }
            else
            {
                logger.InfoFormat("Flight saved: {0}", flight);
            }
        }

    }


    public void Update(int id, Flight flight)
    {
        logger.InfoFormat("Updating flight with ID: {0}", id);

        var con = DBUtils.getConnection(connectionString);

        using (var comm = con.CreateCommand())
        {
            logger.Info("Updating flight...");
            comm.CommandText =
                "UPDATE Flight SET available_seats = @availableSeats WHERE id = @id";
            logger.Info($"SQL Command: {comm.CommandText}");

            var paramId = comm.CreateParameter();
            paramId.ParameterName = "@id";
            paramId.Value = id;
            comm.Parameters.Add(paramId);

            var paramAvailableSeats = comm.CreateParameter();
            paramAvailableSeats.ParameterName = "@availableSeats";
            paramAvailableSeats.Value = flight.AvailableSeats;
            comm.Parameters.Add(paramAvailableSeats);

            var result = comm.ExecuteNonQuery();

            if (result == 0)
            {
                throw new Exception("Flight not updated");
            }
            else
            {
                logger.InfoFormat("Flight updated: {0}", flight);
            }
        }
    }

    public void Delete(int id)
    {
        logger.InfoFormat("Deleting flight with ID: {0}", id);

        var con = DBUtils.getConnection(connectionString);

        using (var comm = con.CreateCommand())
        {
            logger.Info("Deleting flight...");
            comm.CommandText = "DELETE FROM Flight WHERE id = @id";
            logger.Info($"SQL Command: {comm.CommandText}");

            var paramId = comm.CreateParameter();
            paramId.ParameterName = "@id";
            paramId.Value = id;
            comm.Parameters.Add(paramId);

            var result = comm.ExecuteNonQuery();

            if (result == 0)
            {
                throw new Exception("Flight not found");
            }
            else
            {
                logger.InfoFormat("Flight with ID: {0} deleted", id);
            }
        }
    }

    public Flight Find(int id)
    {
        logger.InfoFormat("Finding flight with ID: {0}", id);

        Flight flight = null;
        var con = DBUtils.getConnection(connectionString);

        using (var comm = con.CreateCommand())
        {
            logger.Info("Executing SELECT command to fetch flight...");
            comm.CommandText =
                "SELECT * FROM Flight WHERE id = @id";
            logger.Info($"SQL Command: {comm.CommandText}");

            var paramId = comm.CreateParameter();
            paramId.ParameterName = "@id";
            paramId.Value = id;
            comm.Parameters.Add(paramId);

            using (var reader = comm.ExecuteReader())
            {
                if (reader.Read())
                {
                    string destination = reader.GetString(reader.GetOrdinal("destination"));

                    long departureTimeUnix = reader.GetInt64(reader.GetOrdinal("departure_time"));
                    long arrivalTimeUnix = reader.GetInt64(reader.GetOrdinal("arrival_time"));
                    string airport = reader.GetString(reader.GetOrdinal("airport"));
                    int availableSeats = reader.GetInt32(reader.GetOrdinal("available_seats"));


                    DateTime departureTime = UnixTimestampToDateTime(departureTimeUnix);
                    DateTime arrivalTime = UnixTimestampToDateTime(arrivalTimeUnix);

                    flight = new Flight(destination, departureTime, arrivalTime, airport, availableSeats)
                    {
                        Id = id
                    };
                }
            }
        }

        logger.Info(flight != null ? $"Flight found: {flight}" : "Flight not found");
        return flight;
    }


    public DateTime UnixTimestampToDateTime(long unixTime)
    {
        DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(unixTime);
        return dateTimeOffset.DateTime;
    }


    public IEnumerable<Flight> FindAll()
    {
        var flights = new List<Flight>();

        using (var con = DBUtils.getConnection(connectionString))
        using (var comm = con.CreateCommand())
        {
            comm.CommandText = "SELECT * FROM Flight";

            using (var reader = comm.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32(reader.GetOrdinal("id"));
                    string destination = reader.GetString(reader.GetOrdinal("destination"));

                    long departureTimeUnix = reader.GetInt64(reader.GetOrdinal("departure_time"));
                    long arrivalTimeUnix = reader.GetInt64(reader.GetOrdinal("arrival_time"));
                    string airport = reader.GetString(reader.GetOrdinal("airport"));
                    int availableSeats = reader.GetInt32(reader.GetOrdinal("available_seats"));


                    DateTime departureTime = UnixTimestampToDateTime(departureTimeUnix);
                    DateTime arrivalTime = UnixTimestampToDateTime(arrivalTimeUnix);

                    var flight = new Flight(destination, departureTime, arrivalTime, airport, availableSeats)
                    {
                        Id = id
                    };

                    flights.Add(flight);
                }
            }
        }

        return flights.AsEnumerable();
    }

    public DateTime ConvertUnixTimestampToDateTime(long unixTimestamp)
    {
        var dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(unixTimestamp);
        return dateTimeOffset.DateTime;
    }

    public List<Flight> SearchByDestinationAndDate(string destination, DateTime departureDate)
    {
        var flights = new List<Flight>();

        using (var con = DBUtils.getConnection(connectionString))
        using (var comm = con.CreateCommand())
        {
            long unixStart = ((DateTimeOffset)departureDate.Date).ToUnixTimeMilliseconds();
            long unixEnd = ((DateTimeOffset)departureDate.Date.AddDays(1)).ToUnixTimeMilliseconds();

            comm.CommandText = "SELECT * FROM Flight WHERE LOWER(destination) = LOWER(@destination) AND departure_time >= @start AND departure_time < @end";
        
            comm.Parameters.Add(new SQLiteParameter("@destination", destination));
            comm.Parameters.Add(new SQLiteParameter("@start", unixStart));
            comm.Parameters.Add(new SQLiteParameter("@end", unixEnd));

            using (var reader = comm.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32(reader.GetOrdinal("id"));
                    string dest = reader.GetString(reader.GetOrdinal("destination"));

                    long departureTimeUnix = reader.GetInt64(reader.GetOrdinal("departure_time"));
                    long arrivalTimeUnix = reader.GetInt64(reader.GetOrdinal("arrival_time"));
                    string airport = reader.GetString(reader.GetOrdinal("airport"));
                    int availableSeats = reader.GetInt32(reader.GetOrdinal("available_seats"));

                    DateTime departureTime = UnixTimestampToDateTime(departureTimeUnix);
                    DateTime arrivalTime = UnixTimestampToDateTime(arrivalTimeUnix);

                    var flight = new Flight(dest, departureTime, arrivalTime, airport, availableSeats)
                    {
                        Id = id
                    };

                    flights.Add(flight);
                }
            }
        }

        return flights;
    }
    
    
    public List<Flight> SearchBySeatsAndDestination(string destination, int requiredSeats)
    {
        var flights = new List<Flight>();

        using (var con = DBUtils.getConnection(connectionString))
        using (var comm = con.CreateCommand())
        {
            // Comanda SQL pentru căutarea unui zbor pe baza destinației și a numărului de locuri
            comm.CommandText = "SELECT * FROM Flight WHERE LOWER(destination) = LOWER(@destination) AND available_seats = @requiredSeats";

            // Adăugăm parametrii la interogare
            comm.Parameters.Add(new SQLiteParameter("@destination", destination));
            comm.Parameters.Add(new SQLiteParameter("@requiredSeats", requiredSeats));

            // Executăm interogarea și citim rezultatele
            using (var reader = comm.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32(reader.GetOrdinal("id"));
                    string dest = reader.GetString(reader.GetOrdinal("destination"));

                    long departureTimeUnix = reader.GetInt64(reader.GetOrdinal("departure_time"));
                    long arrivalTimeUnix = reader.GetInt64(reader.GetOrdinal("arrival_time"));
                    string airport = reader.GetString(reader.GetOrdinal("airport"));
                    int availableSeats = reader.GetInt32(reader.GetOrdinal("available_seats"));

                    DateTime departureTime = UnixTimestampToDateTime(departureTimeUnix);
                    DateTime arrivalTime = UnixTimestampToDateTime(arrivalTimeUnix);

                    var flight = new Flight(dest, departureTime, arrivalTime, airport, availableSeats)
                    {
                        Id = id
                    };

                    flights.Add(flight);
                }
            }
        }

        return flights;
    }

}

