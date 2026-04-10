namespace Paps.Persistence
{
    public interface ISerializer
    {
        string Serialize<T>(T obj);
        T Deserialize<T>(string data);
        void Deserialize<T>(string data, ref T container);
    }
}