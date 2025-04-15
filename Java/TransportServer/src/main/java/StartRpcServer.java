import transport.network.utils.AbstractServer;
import transport.network.utils.ServerException;
import transport.network.utils.TransportRpcConcurrentServer;
import transport.persistence.repositoryBD.FlightRepoBD;
import transport.persistence.repositoryBD.TicketRepoBD;
import transport.persistence.repositoryBD.UserRepoBD;
import transport.server.TransportServicesImpl;
import transport.services.ITransportServices;

import java.io.IOException;
import java.util.Properties;

public class StartRpcServer {
    private static int defaultPort=55555;
    public static void main(String[] args) {

        Properties serverProps = new Properties();
        try {
            serverProps.load(StartRpcServer.class.getResourceAsStream("/transportserver.properties"));
            System.out.println("Server properties set. ");
            serverProps.list(System.out);
        } catch (IOException e) {
            System.err.println("Cannot find transportserver.properties "+e);
            return;
        }
        FlightRepoBD flightRepository = new FlightRepoBD(serverProps);
        TicketRepoBD ticketRepository = new TicketRepoBD(serverProps);
        UserRepoBD userRepository = new UserRepoBD(serverProps);

        ITransportServices transportServerImpl = TransportServicesImpl.getInstance(flightRepository, ticketRepository, userRepository);
        int transportServerPort=defaultPort;
        try {
            transportServerPort = Integer.parseInt(serverProps.getProperty("transport.server.port"));
        } catch (NumberFormatException nef){
            System.err.println("Wrong  Port Number"+nef.getMessage());
            System.err.println("Using default port "+defaultPort);
        }
        System.out.println("Starting server on port: " + transportServerPort);
        AbstractServer server = TransportRpcConcurrentServer.getInstance(transportServerPort, transportServerImpl);
        try {
            server.start();
        } catch (ServerException e) {
            System.err.println("Error starting the server" + e.getMessage());
        }finally {
            try {
                server.stop();
            }catch(ServerException e){
                System.err.println("Error stopping server "+e.getMessage());
            }
        }
    }
}