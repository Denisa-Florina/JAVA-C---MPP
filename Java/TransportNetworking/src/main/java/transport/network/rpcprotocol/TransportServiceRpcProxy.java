package transport.network.rpcprotocol;

import transport.model.Flight;
import transport.model.Ticket;
import transport.model.User;
import transport.services.ITransportObserver;
import transport.services.ITransportServices;
import transport.services.TransportException;

import java.io.IOException;
import java.io.ObjectInputStream;
import java.io.ObjectOutputStream;
import java.io.Serializable;
import java.net.Socket;
import java.time.LocalDate;
import java.time.LocalDateTime;
import java.time.format.DateTimeFormatter;
import java.util.List;
import java.util.Map;
import java.util.concurrent.*;

public class TransportServiceRpcProxy implements Serializable, ITransportServices, ITransportObserver {
    private String host;
    private int port;

    private ITransportObserver client;

    private ObjectInputStream input;
    private ObjectOutputStream output;
    private Socket connection;

    private BlockingQueue<Response> qresponses;
    private volatile boolean finished;

    static private TransportServiceRpcProxy instance = null;

    public static TransportServiceRpcProxy getInstance(String host, int port) {
        if (instance == null) {
           instance = new TransportServiceRpcProxy(host, port);
        }
        return instance;
    }

    private TransportServiceRpcProxy(String host, int port) {
        System.out.println("IN TRANSPORT RPC PROXY");
        this.host = host;
        this.port = port;
        qresponses=new LinkedBlockingQueue<Response>();
    }

    @Override
    public void login(String username, String password, ITransportObserver client) {
        System.out.println("IN TRANSPORT RPC PROXY LOGIN");
        try {
            if (connection == null || connection.isClosed()) {
                initializeConnection();
            }
            this.client=client;

            User user = new User(username, password);

            Request req = new Request.Builder().type(RequestType.LOGIN).data(user).build();
            sendRequest(req);
            Response response=readResponse();
            if (response.type()== ResponseType.OK){
                System.out.println("IN TRANSPORT RPC PROXY LOGIN CU SUCCESS");
                return;
            }
            if (response.type()== ResponseType.ERROR){
                String err=response.data().toString();
                closeConnection();
                throw new TransportException(err);
            }
            }
            catch (TransportException e) {
                e.printStackTrace();
            }
    }

    private void initializeConnection() throws TransportException {
        System.out.println("Initializing Transport Proxy connection...");
        try {
            if (connection == null || connection.isClosed()) {
                System.out.println("Connecting to server on port " + port + " and host " + host);
                connection = new Socket(host, port);
            }
            if (output == null) {
                System.out.println("Initializing output stream...");
                output = new ObjectOutputStream(connection.getOutputStream());
                output.flush();
            }
            if (input == null) {
                System.out.println("Initializing input stream...");
                input = new ObjectInputStream(connection.getInputStream());
            }
            //finished = false;
            startReader();
        } catch (IOException e) {
            throw new TransportException("Error connecting to server: " + e.getMessage(), e);
        }
    }

    private void startReader(){
        Thread tw=new Thread(new ReaderThread());
        tw.start();
    }

    @Override
    public void reservationAdded(Object object) throws TransportException {
        client.reservationAdded(object);
    }


    private class ReaderThread implements Runnable {
        public void run() {
            System.out.println("IN TRANSPORT RPC PROXY READER");
            while (!finished) {
                try {
                    Object response=input.readObject();
                    System.out.println("response received "+response);
                    if (isUpdate((Response)response)){
                        System.out.println("RUN PROXY UPDATE");
                        handleUpdate((Response)response);
                    }else{
                        try {
                            qresponses.put((Response)response);
                        } catch (InterruptedException e) {
                            e.printStackTrace();
                        }
                    }
                } catch (IOException e) {
                    System.out.println("Reading error "+e);
                } catch (ClassNotFoundException e) {
                    System.out.println("Reading error "+e);
                }
            }
        }
    }

    private void handleError(String errorMessage) {
        System.out.println("Error: " + errorMessage);
    }


    private void sendRequest(Request request) throws TransportException {
        System.out.println("IN TRANSPORT RPC PROXY SEND REQUEST");
        try {
            System.out.println("A");
            if (connection == null || connection.isClosed()) {
                initializeConnection();
            }
            System.out.println("B");
            output.writeObject(request);
            System.out.println("C");
            output.flush();
            System.out.println("D");
        } catch (IOException e) {
            throw new TransportException("Error sending object: " + e);
        }
    }

    private Response readResponse() throws TransportException {
        Response response=null;
        try{
            response=qresponses.take();
        } catch (InterruptedException e) {
            e.printStackTrace();
        }
        return response;
    }

    private void closeConnection() {
        finished=true;
        try {
            input.close();
            output.close();
            connection.close();
            client=null;
        } catch (IOException e) {
            e.printStackTrace();
        }
    }


    @Override
    public void logout(String username, ITransportObserver client) throws TransportException {
        Request req = new Request.Builder().type(RequestType.LOGOUT).data(username).build();
        sendRequest(req);
        Response response = readResponse();
        closeConnection();
        if (response.type() == ResponseType.ERROR){
            String err = response.data().toString();
            throw new TransportException(err);
        }
    }

    @Override
    public String hashPassword(String password) {
        return "";
    }

    @Override
    public User findUser(String username) {
        return null;
    }

    @Override
    public void addUser(User user) {

    }

    @Override
    public boolean buyTicket(String flight, int seats, List<String> passengers) {
        System.out.println("PROXY Buying ticket for flight: " + flight + " with " + seats + " seats for passengers: " + passengers);
        try {
            String passagersString = flight;
            passagersString += ";" + seats; // Using semicolon as separator
            passagersString += ";" + String.join(",", passengers);

            Object data1 = (String)passagersString;

            Request req = new Request.Builder().type(RequestType.NEW_TICKET)
                    .data(data1)
                    .build();

            System.out.println("SENDING request of type: " + req.type());
            sendRequest(req);

            System.out.println("Astept raspuns");
            Response response = readResponse();

            System.out.println("RESPONSE: " + response);

            if (response.type() == ResponseType.ERROR) {
                throw new TransportException((String) response.data());
            } else if (response.type() == ResponseType.NEW_TICKET) {
                System.out.println("Ticket purchased successfully!");
                return true;
            } else if (response.type() == ResponseType.OK) {
                System.out.println("Ticket purchased successfully!");
                return true;
            } else {
                throw new TransportException("Unexpected response type: " + response.type());
            }

        } catch (TransportException e) {
            e.printStackTrace();
            return false;
        }
    }


    @Override
    public List<String> searchFlights(String destination, LocalDate departure) {
        System.out.println("Searching flights for destination: " + destination + " and departure date: " + departure);

        try {
            Object[] data = {destination, departure};

            Request req = new Request.Builder().type(RequestType.SEARCH_FLIGHT_BY_DESTINATION_DATE)
                    .data(data)
                    .build();

            sendRequest(req);

            Response response = readResponse();
            if (response.type() == ResponseType.ERROR) {
                throw new TransportException((String) response.data());
            }

            if (response.type() == ResponseType.SEARCH_FLIGHT_BY_DESTINATION_DATE) {
                return (List<String>) response.data();
            } else {
                throw new TransportException("Unexpected response type: " + response.type());
            }

        } catch (TransportException e) {
            e.printStackTrace();
            return List.of("Error: " + e.getMessage());
        }
    }


    @Override
    public List<Flight> getAllFlights() {
        System.out.println("IN TRANSPORT RPC PROXY GET ALL FLIGHTS");
        try {
            Request req = new Request.Builder().type(RequestType.GET_ALL_FLIGHTS).build();
            sendRequest(req);
            Response response = readResponse();

            System.out.println("Server response: " + response);

            if (response.type() == ResponseType.ERROR) {
                String err = response.data().toString();
                throw new TransportException(err);
            }

            if (response.data() instanceof List) {
                List<Flight> trips = (List<Flight>) response.data();
                return trips;
            } else {
                throw new TransportException("Expected a list of flights, but got: " + response.data().getClass());
            }
        } catch (TransportException e) {
            e.printStackTrace();
        }
        return null;
    }


    private void handleUpdate(Response response){
        System.out.println("Update");
        if (response.type()== ResponseType.TICKET_ADDED_NOTIFICATION){
            Object message= (Object) response.data();
            try {
                client.reservationAdded(message);

            } catch (TransportException e) {
                e.printStackTrace();
            }
        }
    }

    private boolean isUpdate(Response response){
       return response.type() == ResponseType.TICKET_ADDED_NOTIFICATION;

    }



}
