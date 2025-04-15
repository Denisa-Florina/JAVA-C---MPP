package transport.services;

import transport.model.Ticket;

public interface ITransportObserver {
    void reservationAdded(Object object) throws TransportException;
}
