package transport.client;

import javafx.application.Application;
import javafx.fxml.FXMLLoader;
import javafx.scene.Parent;
import javafx.scene.Scene;
import javafx.stage.Stage;


import transport.client.controllers.LoginController;
import transport.client.controllers.MainController;
import transport.network.rpcprotocol.TransportServiceRpcProxy;
import transport.services.ITransportServices;


import java.io.IOException;
import java.util.Properties;

public class StartRpcClient extends Application {
    private Stage primaryStage;

    private static int defaultChatPort = 55555;
    private static String defaultServer = "localhost";


    public void start(Stage primaryStage) throws Exception {
        System.out.println("In start");
        Properties clientProps = new Properties();
        try {
            clientProps.load(StartRpcClient.class.getResourceAsStream("/transportclient.properties"));
            System.out.println("Client properties set. ");
            clientProps.list(System.out);
        } catch (IOException e) {
            System.err.println("Cannot find transportclient.properties " + e);
            return;
        }
        String serverIP = clientProps.getProperty("transport.server.host", defaultServer);
        int serverPort = defaultChatPort;

        try {
            serverPort = Integer.parseInt(clientProps.getProperty("transport.server.port"));
        } catch (NumberFormatException ex) {
            System.err.println("Wrong port number " + ex.getMessage());
            System.out.println("Using default port: " + defaultChatPort);
        }
        System.out.println("Using server IP " + serverIP);
        System.out.println("Using server port " + serverPort);

        ITransportServices server = TransportServiceRpcProxy.getInstance(serverIP, serverPort);

        System.out.println("Server started");

        FXMLLoader loader = new FXMLLoader(
                getClass().getResource("/view/login.fxml"));
        Parent root=loader.load();


        LoginController ctrl =
                loader.<LoginController>getController();
        ctrl.setServer(server);


        FXMLLoader cloader = new FXMLLoader(
                getClass().getResource("/view/client.fxml"));
        Parent croot=cloader.load();


        MainController mainController =
                cloader.<MainController>getController();
        mainController.setServer(server);

        ctrl.setMainController(mainController);
        ctrl.setParent(croot);

        primaryStage.setTitle("MPP transport app");
        primaryStage.setScene(new Scene(root));
        ctrl.setStage(primaryStage);
        primaryStage.show();

    }


}