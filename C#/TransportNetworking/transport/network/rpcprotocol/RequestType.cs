namespace TransportNetworking.transport.network.rpcprotocol;

[Serializable]
public enum RequestType
{
    LOGIN,
    SEARCH_FLIGHT_BY_DESTINATION_DATE,
    GET_ALL_FLIGHTS,
    SEARCH_FLIGHT_BY_DESTINATION_SEATS,
    NEW_TICKET,
    LOGOUT,
    TICKET_ADDED_NOTIFICATION,
    CLIENT
}