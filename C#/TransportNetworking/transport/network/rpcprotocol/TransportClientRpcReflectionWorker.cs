using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using TransportServices.transport.services;
using JsonSerializer = System.Text.Json.JsonSerializer;
using Newtonsoft.Json;
using Transport.Services;
using TransportModel.transport.model;

namespace TransportNetworking.transport.network.rpcprotocol;

public class TransportClientRpcReflectionWorker : ITransportObserver
{
    private ITransportServices server;
    private Socket connection;

    private StreamReader input;
    private StreamWriter output;
    private volatile bool connected;

    public TransportClientRpcReflectionWorker(ITransportServices server, Socket connection)
    {
        this.server = server;
        this.connection = connection;
        try
        {
            NetworkStream networkStream = new NetworkStream(connection);
            output = new StreamWriter(networkStream);
            input = new StreamReader(networkStream);
            connected = true;
        }
        catch (IOException e)
        {
            Console.WriteLine(e.StackTrace);
        }
    }

    public void Run()
    {
        while (connected)
        {
            try
            {
                string line = input.ReadLine();
                Console.WriteLine(line);
                Request request = JsonConvert.DeserializeObject<Request>(line!);
                //BinaryFormatter formatter = new BinaryFormatter();
                //object request = formatter.Deserialize(input.BaseStream);
                Console.WriteLine("RUN:" + request);
                Response response = HandleRequest(request);
                if (response != null)
                {
                    SendResponse(response);
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e.StackTrace);
            }

            Thread.Sleep(1000);
        }

        try
        {
            input.Close();
            output.Close();
            connection.Close();
        }
        catch (IOException e)
        {
            Console.WriteLine("Error " + e);
        }
    }
    
    
    private void SendResponse(Response response)
    {
        //Console.WriteLine("sending response " + response);
        lock (output)
        {
            string responseString = JsonConvert.SerializeObject(response);
            output.WriteLine(responseString);
            output.Flush();
            //BinaryFormatter formatter = new BinaryFormatter();
            /*using (MemoryStream ms = new MemoryStream())
            {
                formatter.Serialize(ms, response);
                byte[] buffer = ms.ToArray();
                output.Write(buffer, 0, buffer.Length);
                output.Flush();
            }*/
        }
    }

    
    //asta ii pentru update
    public void ReservationAdded(Object obj)
    {
        Response resp = new Response.Builder().Type(ResponseType.TICKET_ADDED_NOTIFICATION).Data(obj).Build();
        Console.WriteLine("Reservation added: " + obj);
        try
        {
            SendResponse(resp);
        }
        catch (IOException e)
        {
            Console.WriteLine(e.StackTrace);
        }
    }

    private static Response okResponse = new Response.Builder().Type(ResponseType.OK).Build();

    private Response HandleRequest(Request request)
    {
        Response response = null;
        string handlerName = "Handle" + request.Type;
        Console.WriteLine("HandlerName " + handlerName);
        try
        {
            MethodInfo method = this.GetType().GetMethod(handlerName, new Type[] { typeof(Request) });
            if (method != null)
            {
                response = (Response)method.Invoke(this, new object[] { request });
                //Console.WriteLine("Method " + handlerName + " invoked");
            }
            else
            {
                Console.WriteLine("Method " + handlerName + " not found");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.StackTrace);
        }

        return response;
    }

    public Response HandleGET_ALL_FLIGHTS(Request request)
    {
        Console.WriteLine("Handling GET_ALL_FLIGHTS");
        
        try
        {
            //Console.WriteLine("Trimit avioanele");
            return new Response.Builder().Type(ResponseType.GET_ALL_FLIGHTS).Data(server.GetAllFlights()).Build();
        }
        catch (TransportException e)
        {
            return new Response.Builder().Type(ResponseType.ERROR).Data(e.Message).Build();
        }
        
        
    }

    public Response HandleLOGIN(Request request)
    {
        Console.WriteLine("Login Request ... " + request.Type);
        
        if (request.Data == null)
        {
            return new Response.Builder().Type(ResponseType.ERROR).Data("Request data is null").Build();
        }

        User user = request.Data as User; 
        if (user == null)
        {

            return new Response.Builder().Type(ResponseType.ERROR).Data("Invalid user data").Build();
        }

        try
        {
            //Console.WriteLine("in HandleLOGIN: ");
            Boolean succes = server.Login(user.Username, user.Password, this);

            if (succes == true){
                Console.WriteLine("Login successful! (Worker)");
                return okResponse;
            }
            else
            {
                return new Response.Builder().Type(ResponseType.ERROR).Data("").Build();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.StackTrace);
            connected = false;
            return new Response.Builder().Type(ResponseType.ERROR).Data(e.Message).Build();
        }

    }

    public Response HandleSEARCH_FLIGHT_BY_DESTINATION_DATE(Request request)
    {
        Object[] data = (Object[])request.Data;
        string destination = (string) data[0];
        DateTime departureDate = (DateTime) data[1];
        
        try {
            List<Flight> flightDetails = server.SearchFlights(destination, departureDate);
            return new Response.Builder().Type(ResponseType.SEARCH_FLIGHT_BY_DESTINATION_DATE).Data(flightDetails).Build();
        } catch (TransportException e) {
            return new Response.Builder().Type(ResponseType.ERROR).Data(e.Message).Build();
        }
    }

    public Response HandleNEW_TICKET(Request request)
    {
        String data = (string)request.Data;

        List<string> obj = data.Split(';')
            .Select(s => s.Trim())
            .ToList();
        
        List<string> passagersList = obj[3].Split(',')
            .Select(s => s.Trim())
            .ToList();

        Boolean success = server.BuyTicket(obj[0], int.Parse(obj[1]),int.Parse(obj[2]), passagersList);

        if (success) {
            return okResponse;
        } else {
            return new Response.Builder()
                .Type(ResponseType.ERROR)
                .Data("Something went bad.")
                .Build();
        }
    }
}