package transport.persistence;

public interface Repository<ID, T> {
    void add(T elem);
    void update(ID id,T elem);
    void delete(ID id);
    T find(ID id);
    Iterable<T> findAll();
}
