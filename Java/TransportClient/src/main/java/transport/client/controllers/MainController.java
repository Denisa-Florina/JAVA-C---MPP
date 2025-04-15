package transport.client.controllers;

import javafx.application.Platform;
import javafx.collections.FXCollections;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;
import javafx.scene.control.*;
import javafx.stage.Stage;
import transport.model.Flight;
import transport.model.Ticket;
import transport.model.User;
import transport.services.ITransportObserver;
import transport.services.ITransportServices;
import transport.services.TransportException;

import java.io.Serializable;
import java.net.URL;
import java.util.Arrays;
import java.util.List;
import java.util.ResourceBundle;
import java.util.stream.Collectors;
import java.util.stream.StreamSupport;

public class MainController implements Serializable, Initializable, ITransportObserver {
    private ITransportServices service;
    private Stage stage;
    User loggedUser;

    @FXML private ListView<String> flightsList;
    @FXML private TextField destinationField;
    @FXML private DatePicker datePicker;
    @FXML private Spinner<Integer> seatsSpinner;
    @FXML private TextField passengersField;
    @FXML private Button searchButton;
    @FXML private Button buyButton;
    @FXML private Button logoutButton;



    public MainController() {
        System.out.println("MainController constructor");
    }

    public void setServer(final ITransportServices service) throws TransportException {
        if (service == null) {
            System.out.println("Server is null!");
        } else {
            System.out.println("Server initialized: " + service);
        }
        this.service = service;

       update();

    }

    public void setStage(Stage stage) {
        System.out.println("Initialized Stage");
        this.stage = stage;
    }

    @FXML
    public void initialize(URL url, ResourceBundle resourceBundle) {
        System.out.println("initialize");
        seatsSpinner.setValueFactory(new SpinnerValueFactory.IntegerSpinnerValueFactory(1, 10, 1));

    }

    public void setLoggedUser(User loggedUser) {
        System.out.println("Initialized LoggedUser");
        this.loggedUser = loggedUser;
    }


    @FXML
    public void handleSearch() {
        String destination = destinationField.getText();
        if (destination.isEmpty() || datePicker.getValue() == null) {
            showAlert("Error", "Destination and date are required!");
            return;
        }

        List<String> flights = null;
        try {
            flights = service.searchFlights(destination, datePicker.getValue());
        } catch (TransportException e) {
            showAlert("Error", "Failed to search flights: " + e.getMessage());
            return;
        }

        flightsList.getItems().setAll(flights);
    }



    @FXML
    public void handleBuy() {
        String selectedFlight = flightsList.getSelectionModel().getSelectedItem();
        if (selectedFlight == null) {
            showAlert("Error", "Select a flight!");
            return;
        }

        int seats = seatsSpinner.getValue();
        List<String> passengers = Arrays.asList(passengersField.getText().split(","));
        if (passengers.isEmpty()) {
            showAlert("Error", "Enter passenger names!");
            return;
        }

        boolean success = service.buyTicket(selectedFlight, seats, passengers);
        if (success) {
            showAlert("Success", "Ticket purchased successfully!");
        } else {
            showAlert("Error", "Not enough seats available!");
        }
    }

    @FXML
    public void handleLogout() {
        stage.close();
    }

    private void showAlert(String title, String message) {
        Alert alert = new Alert(Alert.AlertType.INFORMATION);
        alert.setTitle(title);
        alert.setContentText(message);
        alert.showAndWait();
    }


    private void update() {
        try {
            List<Flight> flights = service.getAllFlights();
            List<String> flightDisplayData = flights.stream()
                    .map(flight -> flight.getDestination() + " "+ flight.getDepartureTime().toLocalDate() + " " + flight.getDepartureTime().toLocalTime() +
                            " Seats: " + flight.getAvailableSeats())
                    .collect(Collectors.toList());

            flightsList.setItems(FXCollections.observableArrayList(flightDisplayData));
        } catch (TransportException e) {
            System.out.println(e);
        }
    }


    @Override
    public void reservationAdded(Object obj) {
        Platform.runLater(() -> {
            System.out.println("NOTIFY");
            update();
        });

    }
}