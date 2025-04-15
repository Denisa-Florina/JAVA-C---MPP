package transport.client.controllers;

import javafx.event.EventHandler;
import javafx.fxml.FXML;
import javafx.fxml.FXMLLoader;
import javafx.scene.Node;
import javafx.scene.Parent;
import javafx.scene.Scene;
import javafx.scene.control.*;
import javafx.event.ActionEvent;

import javafx.scene.control.TextField;
import javafx.stage.Stage;
import javafx.stage.WindowEvent;
import transport.model.Ticket;
import transport.model.User;
import transport.services.ITransportObserver;
import transport.services.ITransportServices;
import transport.services.TransportException;


import java.io.FileReader;
import java.io.IOException;
import java.util.Properties;

public class LoginController {

    @FXML private TextField usernameField;
    @FXML private PasswordField passwordField;

    private MainController mainController1;
    private ITransportServices service;
    private Stage stage;
    private Parent mainParent;
    private User loggedUser;

    public void setServer(final ITransportServices service) {
        this.service = service;
    }

    public void setMainController(final MainController mainController) {
        this.mainController1 = mainController;
    }
    public void setParent(Parent parent){
        mainParent = parent;
    }
    public void setStage(Stage stage) {
        this.stage = stage;
    }

    @FXML
    public void initialize() {
    }

    @FXML
    private void handleLogin() {
        String username = usernameField.getText();
        String password = passwordField.getText();

        if (username.isEmpty() || password.isEmpty()) {
            showAlert("Error", "Both fields are required!");
            return;
        }

        loggedUser = new User(username, password);

        try{
            mainController1.setLoggedUser(loggedUser);
            service.login(username, password, mainController1);
            Stage stage2 = new Stage();
            stage2.setTitle("Transport Window for " + loggedUser.getUsername());
            stage2.setScene(new Scene(mainParent));
            stage2.setOnCloseRequest(new EventHandler<WindowEvent>() {
                @Override
                public void handle(WindowEvent event) {
                    mainController1.handleLogout();
                    System.exit(0);
                }
            });
            stage2.show();
            stage.close();
        } catch (Exception e) {
            showAlert("Error", e.getMessage());
        }
    }

    private void showAlert(String title, String message) {
        Alert alert = new Alert(Alert.AlertType.ERROR);
        alert.setTitle(title);
        alert.setContentText(message);
        alert.showAndWait();
    }

    private void showAlertSucces(String title, String message) {
        Alert alert = new Alert(Alert.AlertType.INFORMATION);
        alert.setTitle(title);
        alert.setContentText(message);
        alert.showAndWait();
    }


    @FXML
    private void handleRegister() {
        String username = usernameField.getText();
        String password = passwordField.getText();

        if (username.isEmpty() || password.isEmpty()) {
            showAlert("Error", "Both fields are required for registration!");
            return;
        }
        User existingUser = service.findUser(username);
        if (existingUser != null) {
            showAlert("Error", "User already exists!");
            return;
        }

        String hashedPassword = service.hashPassword(password);
        User newUser = new User(username, hashedPassword);
        service.addUser(newUser);
        showAlertSucces("Success", "User registered successfully!");

    }
}
