package transport.network.rpcprotocol;

import transport.model.Flight;
import transport.model.Ticket;
import transport.model.User;
import transport.services.*;

import java.io.IOException;
import java.io.ObjectInputStream;
import java.io.ObjectOutputStream;
import java.lang.reflect.InvocationTargetException;
import java.lang.reflect.Method;
import java.net.Socket;
import java.time.LocalDate;
import java.time.LocalDateTime;
import java.time.format.DateTimeFormatter;
import java.util.Arrays;
import java.util.List;
import java.util.stream.Collectors;

public class TransportClientRpcReflectionWorker implements Runnable, ITransportObserver {
    private ITransportServices server;
    private Socket connection;
    ITransportObserver client;

    private ObjectInputStream input;
    private ObjectOutputStream output;
    private volatile boolean connected;

    public TransportClientRpcReflectionWorker(ITransportServices server, Socket connection) {
        System.out.println("IN TRANSPORT WORKER CONSTRUCTOR");
        this.server = server;
        this.connection = connection;
        try {
            if (connection != null) {
                System.out.println("IN TRANSPORT WORKER CONNECTION conectiune nenula");
                output = new ObjectOutputStream(connection.getOutputStream());
                output.flush();
                input = new ObjectInputStream(connection.getInputStream());
                connected = true;
            } else {
                System.out.println("IN TRANSPORT WORKER CONNECTION conectiune nenula");
                throw new IOException("Socket connection is null.");
            }
        } catch (IOException e) {
            e.printStackTrace();
            connected = false;
        }
    }

    public void run() {
        System.out.println("IN TRANSPORT WORKER RUN");
        while (connected) {
            System.out.println("IN TRANSPORT WORKER RUN");
            try {
                Object request = input.readObject();
                System.out.println("Response primit din handleRequest.... ");
                Response response = handleRequest((Request) request);
                System.out.println("Response primit din handleRequest: " + response);
                if (response != null) {
                    sendResponse(response);
                }
            } catch (IOException e) {
                e.printStackTrace();
            } catch (ClassNotFoundException e) {
                e.printStackTrace();
            }
            try {
                Thread.sleep(1000);
            } catch (InterruptedException e) {
                e.printStackTrace();
            }
        }
        try {
            input.close();
            output.close();
            connection.close();
        } catch (IOException e) {
            System.out.println("Error " + e);
        }
    }

    private static Response okResponse = new Response.Builder().type(ResponseType.OK).build();

    private Response handleRequest(Request request) {
        System.out.println("IN TRANSPORT WORKER HANDLE REQUEST");
        Response response = null;
        String handlerName = "handle" + (request).type();
        System.out.println("HandlerName " + handlerName);
        try {
            Method method = this.getClass().getDeclaredMethod(handlerName, Request.class);
            response = (Response) method.invoke(this, request);
            System.out.println("Method " + handlerName + " invoked");
        } catch (NoSuchMethodException e) {
            e.printStackTrace();
        } catch (InvocationTargetException e) {
            e.printStackTrace();
        } catch (IllegalAccessException e) {
            e.printStackTrace();
        }

        return response;
    }

    private void sendResponse(Response response) throws IOException {
        System.out.println("IN TRANSPORT WORKER SEND RESPONSE");
        System.out.println("sending response " + response);
        synchronized (output) {
            output.writeObject(response);
            output.flush();
        }
    }

    private Response handleLOGIN(Request request) {
        System.out.println("Login Request ... " + request.type());
        User user = (User) request.data();
        try {
            server.login(user.getUsername(), user.getPassword(), this);
            return okResponse;
        } catch (TransportException e) {
            connected=false;
            return new Response.Builder().type(ResponseType.ERROR).data(e.getMessage()).build();
        }
    }


    private Response handleGET_ALL_FLIGHTS(Request request) {
        System.out.println("Get All Trips Request ...");
        try {
            List<Flight> allFlights = server.getAllFlights();
            return new Response.Builder().type(ResponseType.GET_ALL_FLIGHTS).data(allFlights).build();
        } catch (TransportException e) {
            e.printStackTrace();
            return new Response.Builder().type(ResponseType.ERROR).data(e.getMessage()).build();
        }

    }

    private Response handleSEARCH_FLIGHT_BY_DESTINATION_DATE(Request request) {
        System.out.println("Search flights by destination and date request...");

        Object[] data = (Object[]) request.data();
        String destination = (String) data[0];
        LocalDate departureDate = (LocalDate) data[1];

        try {
            List<String> flightDetails = server.searchFlights(destination, departureDate);  // Ar trebui să returneze deja lista de String-uri
            return new Response.Builder().type(ResponseType.SEARCH_FLIGHT_BY_DESTINATION_DATE).data(flightDetails).build();
        } catch (TransportException e) {
            return new Response.Builder().type(ResponseType.ERROR).data(e.getMessage()).build();
        }
    }


    private Response handleLOGOUT(Request request) {
        System.out.println("Logout Request...");
        User user = (User) request.data();
        try {
            server.logout(user.getUsername(), this);
            connected = false;
            return okResponse;
        } catch (TransportException e) {
            return new Response.Builder()
                    .type(ResponseType.ERROR)
                    .data(e.getMessage())
                    .build();
        }
    }

    private Response handleNEW_TICKET(Request request) {
        System.out.println("Buy Ticket Request... worker");
        String data = (String) request.data();

        List<String> obj = Arrays.stream(data.split(";"))
                .map(String::trim)
                .collect(Collectors.toList());

        System.out.println(data);

        List<String> passagersList = Arrays.stream(obj.get(2).split(","))
                .map(String::trim)
                .collect(Collectors.toList());

        System.out.println(obj.get(0));
        System.out.println(passagersList);
        System.out.println(obj.get(1));

        boolean success = server.buyTicket(obj.get(0), Integer.parseInt(obj.get(1)), passagersList);

        if (success) {
            return okResponse;
        } else {
            return new Response.Builder()
                    .type(ResponseType.ERROR)
                    .data("Something went bad.")
                    .build();
        }


    }

    public void handleTICKET_ADDED_NOTIFICATION(Request request) {
        Response resp = new Response.Builder()
                .type(ResponseType.TICKET_ADDED_NOTIFICATION)
                .data(this)
                .build();
        try {
            sendResponse(resp);
        }
        catch (Exception e) {
            e.printStackTrace();
        }
    }

    public void handleCLIENT(Request request) {
        ITransportObserver observer = (ITransportObserver) request.data();
        this.client = observer;

        Response resp = new Response.Builder()
                .type(ResponseType.CLIENT)
                .data("ok")
                .build();
        try {
            sendResponse(resp);
        }
        catch (Exception e) {
            e.printStackTrace();
        }
    }

    @Override
    public void reservationAdded(Object object) {
        Response resp = new Response.Builder().type(ResponseType.TICKET_ADDED_NOTIFICATION).data(object).build();
        System.out.println("Reservation added: ");
        try {
            sendResponse(resp);
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

}
