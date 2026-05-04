namespace TournemantManager.Contracts;

public interface IStorage<T>
{
    void Save(T item);
    T Load();
}