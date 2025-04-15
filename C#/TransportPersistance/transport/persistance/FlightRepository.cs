
using proiect.repository;
using TransportModel.transport.model;

namespace transport.persistance;

public interface IFlightRepository : IRepository<int, Flight>
{
}