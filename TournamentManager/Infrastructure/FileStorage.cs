using System.IO;
using System.Text.Json;
using TournemantManager.Contracts;

namespace TournemantManager.Infrastructure;

public class FileStorage<T> : IStorage<T>
{
    string _filePath;
    public FileStorage(string filePath)
    {
        _filePath = filePath;
    }

    public void Save(T item)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        string json = JsonSerializer.Serialize(item, options);
        File.WriteAllText(_filePath, json);
    }

    public T Load()
    {
        if (!File.Exists(_filePath))
        {
            return default(T);
        }
        string json = File.ReadAllText(_filePath);
        return JsonSerializer.Deserialize<T>(json);
    }
}