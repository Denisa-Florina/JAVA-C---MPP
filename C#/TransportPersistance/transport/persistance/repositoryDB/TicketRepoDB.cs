using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using log4net;
using proiect.repository;
using tasks.repository;
using TransportModel.transport.model;

public class TicketRepoDB : ITicketRepository
    {
        private static readonly ILog logger = LogManager.GetLogger("TicketRepoDB");
        private readonly IDictionary<string, string> connectionString;

        public TicketRepoDB(IDictionary<string, string> connectionString)
        {
            logger.Info("Initializing TicketRepoDB");
            this.connectionString = connectionString;
        }

        public void Add(Ticket ticket)
        {
            logger.InfoFormat("Saving ticket: {0}", ticket);

            logger.Info("Connecting to database...");
            logger.Info($"Using connection string: {connectionString["ConnectionString"]}");
            var con = DBUtils.getConnection(connectionString);

            using (var comm = con.CreateCommand())
            {
                logger.Info("Inserting ticket...");
                comm.CommandText = "INSERT INTO Ticket (passager, seat_count, idFlight) VALUES (@passager, @seatCount, @idFlight)";
                logger.Info($"SQL Command: {comm.CommandText}");

                var paramPassager = comm.CreateParameter();
                paramPassager.ParameterName = "@passager";
                paramPassager.Value = string.Join(",", ticket.Passager);
                comm.Parameters.Add(paramPassager);

                var paramSeatCount = comm.CreateParameter();
                paramSeatCount.ParameterName = "@seatCount";
                paramSeatCount.Value = ticket.SeatCount;
                comm.Parameters.Add(paramSeatCount);
                
                var paramidFlight = comm.CreateParameter();
                paramidFlight.ParameterName = "@idFlight";
                paramidFlight.Value = ticket.FlightId;
                comm.Parameters.Add(paramidFlight);

                var result = comm.ExecuteNonQuery();

                if (result == 0)
                {
                    throw new Exception("Ticket not saved");
                }
                else
                {
                    logger.InfoFormat("Ticket saved: {0}", ticket);
                }
            }
        }

        public void Update(int id, Ticket ticket)
        {
            logger.InfoFormat("Updating ticket with ID: {0}", id);

            var con = DBUtils.getConnection(connectionString);

            using (var comm = con.CreateCommand())
            {
                logger.Info("Updating ticket...");
                comm.CommandText = "UPDATE Ticket SET passager = @passager, seat_count = @seatCount WHERE id = @id";
                logger.Info($"SQL Command: {comm.CommandText}");

                var paramId = comm.CreateParameter();
                paramId.ParameterName = "@id";
                paramId.Value = id;
                comm.Parameters.Add(paramId);

                var paramPassager = comm.CreateParameter();
                paramPassager.ParameterName = "@passager";
                paramPassager.Value = string.Join(",", ticket.Passager);  // Store list as a comma-separated string
                comm.Parameters.Add(paramPassager);

                var paramSeatCount = comm.CreateParameter();
                paramSeatCount.ParameterName = "@seatCount";
                paramSeatCount.Value = ticket.SeatCount;
                comm.Parameters.Add(paramSeatCount);

                var result = comm.ExecuteNonQuery();

                if (result == 0)
                {
                    throw new Exception("Ticket not updated");
                }
                else
                {
                    logger.InfoFormat("Ticket updated: {0}", ticket);
                }
            }
        }

        public void Delete(int id)
        {
            logger.InfoFormat("Deleting ticket with ID: {0}", id);

            var con = DBUtils.getConnection(connectionString);

            using (var comm = con.CreateCommand())
            {
                logger.Info("Deleting ticket...");
                comm.CommandText = "DELETE FROM Ticket WHERE id = @id";
                logger.Info($"SQL Command: {comm.CommandText}");

                var paramId = comm.CreateParameter();
                paramId.ParameterName = "@id";
                paramId.Value = id;
                comm.Parameters.Add(paramId);

                var result = comm.ExecuteNonQuery();

                if (result == 0)
                {
                    throw new Exception("Ticket not found");
                }
                else
                {
                    logger.InfoFormat("Ticket with ID: {0} deleted", id);
                }
            }
        }

        public Ticket Find(int id)
        {
            logger.InfoFormat("Finding ticket with ID: {0}", id);

            Ticket ticket = null;
            var con = DBUtils.getConnection(connectionString);

            using (var comm = con.CreateCommand())
            {
                logger.Info("Executing SELECT command to fetch ticket...");
                comm.CommandText = "SELECT id, passager, seat_count, idFlight FROM Ticket WHERE id = @id";
                logger.Info($"SQL Command: {comm.CommandText}");

                var paramId = comm.CreateParameter();
                paramId.ParameterName = "@id";
                paramId.Value = id;
                comm.Parameters.Add(paramId);

                using (var reader = comm.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var passager = reader["passager"].ToString().Split(',');
                        ticket = new Ticket(Convert.ToInt32(reader["seat_count"]), new List<string>(passager), Convert.ToInt32(reader["idFlight"]))
                        {
                            Id = Convert.ToInt32(reader["id"])  
                        };
                    }
                }
            }

            logger.Info(ticket != null ? $"Ticket found: {ticket}" : "Ticket not found");
            return ticket;
        }

        public IEnumerable<Ticket> FindAll()
        {
            logger.Info("Retrieving all tickets...");
            var tickets = new List<Ticket>();

            using (var con = DBUtils.getConnection(connectionString))
            using (var comm = con.CreateCommand())
            {
                comm.CommandText = "SELECT id, passager, seat_count, idFlight FROM Ticket";

                using (var reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var passagerValue = reader["passager"];
                        var passager = passagerValue == DBNull.Value ? new string[0] : passagerValue.ToString().Split(',');

                        var ticket = new Ticket(Convert.ToInt32(reader["seat_count"]), new List<string>(passager), Convert.ToInt32(reader["idFlight"]))
                        {
                            Id = Convert.ToInt32(reader["id"])
                        };

                        tickets.Add(ticket);
                    }
                }
            }

            logger.Info($"Found {tickets.Count} tickets.");
            return tickets.AsEnumerable();
        }


}
