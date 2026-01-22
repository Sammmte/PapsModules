using Unity.Serialization.Json;

namespace Paps.UnityPrefs
{
    internal class JsonSerializer : ISerializer
    {
        public T Deserialize<T>(string serialized)
        {
            return JsonSerialization.FromJson<T>(serialized, new JsonSerializationParameters()
            {
                SerializedType = typeof(T)
            });
        }

        public string Serialize<T>(T obj)
        {
            return JsonSerialization.ToJson<T>(obj, new JsonSerializationParameters()
            {
                SerializedType = typeof(T),
                Minified = true
            });
        }
    }
}
