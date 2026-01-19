namespace Paps.UnityPrefs
{
    internal interface ISerializer
    {
        public string Serialize<T>(T obj);
        public T Deserialize<T>(string serialized);
    }
}
