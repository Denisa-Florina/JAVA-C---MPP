using System.Collections.Concurrent;
using System.Net.Sockets;
using Newtonsoft.Json;
using Transport.Services;
using TransportModel.transport.model;
using TransportServices.transport.services;

namespace TransportNetworking.transport.network.rpcprotocol;

public class TransportServicesRpcProxy : ITransportServices
{
    private string host;
    private int port;

    private ITransportObserver client;

    private StreamReader input;
    private StreamWriter output;
    private Socket connection;

    private BlockingCollection<Response> qresponses;
    private volatile bool finished;

    public TransportServicesRpcProxy(string host, int port)
    {
        this.host = host;
        this.port = port;
        qresponses = new BlockingCollection<Response>();
    }
    
    private void CloseConnection()
        {
            finished = true;
            try
            {
                input.Close();
                output.Close();
                connection.Close();
                client = null;
            }
            catch (IOException e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }

        private void SendRequest(Request request)
        {
            try
            {
                if (output == null)
                {
                    throw new InvalidOperationException("Output stream is not initialized.");
                }
                
                Console.WriteLine(request.Type);
                string json = JsonConvert.SerializeObject(request);
                output.WriteLine(json);
                output.Flush();
            }
            catch (IOException e)
            {
                throw new TransportException("Error sending object " + e);
            }
        }

        private Response ReadResponse()
        {
            try
            {
                //qresponses.TryDequeue(out response);
                return qresponses.Take();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                throw new TransportException("Error reading response " + e);
            }
        }
        
        private void InitializeConnection()
        {
            try
            {
                Console.WriteLine("Connecting to server on port " + port + " and host " + host);
                connection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                connection.Connect(host, port);
                output = new StreamWriter(new NetworkStream(connection));
                Console.WriteLine("Connected to server: " + connection + " output: " + output);
                input = new StreamReader(new NetworkStream(connection));
                finished = false;
                StartReader();
            }
            catch (SocketException e)
            {
                Console.WriteLine("Failed to connect to server: " + e.Message);
            }
            catch (IOException e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }

        private void StartReader()
        {
            Thread tw = new Thread(new ReaderThread(this).Run);
            tw.Start();
        }

        private void HandleUpdate(Response response)
        {
            if (response.Type == ResponseType.TICKET_ADDED_NOTIFICATION)
            {
                Object message = response.Data;
                try
                {
                    client.ReservationAdded(message);
                }
                catch (TransportException e)
                {
                    Console.WriteLine(e.StackTrace);
                }
            }
        }

        private bool IsUpdate(Response response)
        {
            return response.Type == ResponseType.TICKET_ADDED_NOTIFICATION;
        }

        private class ReaderThread
        {
            private TransportServicesRpcProxy outerClass;

            public ReaderThread(TransportServicesRpcProxy outerClass)
            {
                this.outerClass = outerClass;
            }

            public void Run()
            {
                //BinaryFormatter formatter = new BinaryFormatter();
                
                while (!outerClass.finished)
                {
                    try
                    {
                        string line = outerClass.input.ReadLine();
                        //Console.WriteLine("Line in Proxy: " + line);
                        if (line == null)
                            throw new IOException("Connection lost!");
                        Response response = JsonConvert.DeserializeObject<Response>(line!);
                        //Console.WriteLine("Response Received in Proxy: " + response);
                        if (outerClass.IsUpdate(response))
                        {
                            outerClass.HandleUpdate(response);
                        }
                        else
                        {
                            try
                            {
                                outerClass.qresponses.Add(response);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.StackTrace);
                            }
                        }
                    }
                    catch (SocketException e)
                    {
                        Console.WriteLine("Connection error: " + e);
                        break;
                    }
                    catch (IOException e)
                    {
                        Console.WriteLine("Reading error " + e);
                        break;
                    }
                }
            }
        }

        public bool Login(string username, string password, ITransportObserver client)
        {
            InitializeConnection();
            
            User user = new User(username, password);

            Request req = new Request.Builder().Type(RequestType.LOGIN).Data(user).Build();
            SendRequest(req);

            Response response = ReadResponse();
            
            if (response.Type == ResponseType.OK)
            {
                Console.WriteLine("Login successful");
                this.client = client;
                return true;
            }
            
            if (response.Type == ResponseType.ERROR)
            {
                Console.WriteLine("Login failed");
                string err = response.Data.ToString();
                CloseConnection();
                return false;
            }
            
            return false;
        }
        

        public List<Flight> GetAllFlights()
        {
            
            Console.WriteLine("Getting all flights in Proxy");

            try
            {
                Console.WriteLine("Trimit request pentru all flights");
                Request req2 = new Request.Builder().Type(RequestType.GET_ALL_FLIGHTS).Build();
                SendRequest(req2);
                Console.WriteLine("Astept rapuns pentru get all flights");
                Response response = ReadResponse();

                Console.WriteLine("Server response: " + response);

                if (response.Type == ResponseType.ERROR)
                {
                    string err = response.Data.ToString();
                    throw new TransportException(err);
                }

                Console.WriteLine("Response data: " + response.Data.GetType());
                List<Flight> trips = (List<Flight>)response.Data;
                return trips;

            }
            catch (TransportException e)
            {
                throw new TransportException(e.Message);
            }
        }

        public bool BuyTicket(string destination, int requiredSeats, int seats, List<string> passengers)
        {
            string passagersString = destination;
            passagersString += ";" + requiredSeats;
            passagersString += ";" + seats;
            passagersString += ";" + string.Join(",", passengers);
            
            Object data1 = (String)passagersString;
            
            Request req = new Request.Builder().Type(RequestType.NEW_TICKET)
                .Data(data1)
                .Build();

            Console.WriteLine("SENDING request of type: ");
            SendRequest(req);

            Console.WriteLine("Astept raspuns");
            Response response = ReadResponse();

            Console.WriteLine("RESPONSE: " + response);
            
            if (response.Type == ResponseType.ERROR) {
                throw new TransportException((String) response.Data);
            } else if (response.Type == ResponseType.NEW_TICKET) {
                Console.WriteLine("Ticket purchased successfully!");
                return true;
            } else if (response.Type == ResponseType.OK) {
                Console.WriteLine("Ticket purchased successfully!");
                return true;
            } else {
                throw new TransportException("Unexpected response type: " + response.Type);
            }
        }

        public List<Flight> SearchFlights(string destination, DateTime departure)
        {
            Console.WriteLine("Searching flights in Proxy");
            Object[] data = [destination, departure];

            Request req = new Request.Builder().Type(RequestType.SEARCH_FLIGHT_BY_DESTINATION_DATE)
                .Data(data)
                .Build();

            SendRequest(req);

            Response response = ReadResponse();
            
            if (response.Type == ResponseType.ERROR) {
                throw new TransportException((string)response.Data);
            }

            if (response.Type == ResponseType.SEARCH_FLIGHT_BY_DESTINATION_DATE) {
                List<Flight> trips = (List<Flight>)response.Data;
                return trips;
            } else {
                throw new TransportException("Unexpected response type: " + response.Type);
            }
            
        }
}