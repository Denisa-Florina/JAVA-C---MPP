namespace proiect.repository;

public interface IRepository<ID, T> 
{
    void Add(T elem);
    void Update(ID id, T elem);
    void Delete(ID id);
    T Find(ID id);
    IEnumerable<T> FindAll();
}