using System.Configuration;
using proiect.repository.repositoryDB;
using TransportNetworking.transport.network.utils;
using TransportServices.transport.services;

namespace TransportServer
{
    public class StartRpcServer
    {
        private static int defaultPort = 55555;
        private static String defaultId = "127.0.0.1";


        static string GetConnectionStringByName(string name)
        {
            string returnValue = null;

            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[name];

            if (settings != null)
                returnValue = settings.ConnectionString;

            return returnValue;
        }

        //[STAThread]
        public static void Main(string[] args)
        {
            IDictionary<string, string> props = new Dictionary<string, string>();

            props.Add("ConnectionString", GetConnectionStringByName("flightDB"));

            FlightRepoDB db = new FlightRepoDB(props);
            TicketRepoDB ticketRepoDB = new TicketRepoDB(props);
            UserRepoDB userRepoDB = new UserRepoDB(props);
            ITransportServices transportServerImpl = new TransportServicesImpl(db, ticketRepoDB, userRepoDB);

            int transportServerPort = defaultPort;
            try
            {
                transportServerPort = int.Parse(ConfigurationManager.AppSettings["TransportServer.Port"]);
            }
            catch (FormatException nef)
            {
                Console.Error.WriteLine("Wrong Port Number" + nef.Message);
                Console.Error.WriteLine("Using default port " + defaultPort);
            }

            Console.WriteLine("Starting server on port: " + transportServerPort);
            AbstractServer server = new TransportRpcConcurrentServer(transportServerPort, transportServerImpl);
            try
            {
                server.Start();
            }
            catch (ServerException e)
            {
                Console.Error.WriteLine("Error starting the server" + e.Message);
            }
            finally
            {
                try
                {
                    server.Stop();
                }
                catch (ServerException e)
                {
                    Console.Error.WriteLine("Error stopping server " + e.Message);
                }
            }
        }
    }
}