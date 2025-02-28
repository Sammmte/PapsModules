using Unity.Serialization.Json;

namespace Paps.Persistence
{
    public class UnityJsonSerializer : ISerializer
    {
        private readonly bool _minified;

        public UnityJsonSerializer(bool minified = false)
        {
            _minified = minified;
        }

        public string Serialize<T>(T obj)
        {
            return JsonSerialization.ToJson(obj, new JsonSerializationParameters()
            {
                Minified = _minified
            });
        }

        public T Deserialize<T>(string data)
        {
            return JsonSerialization.FromJson<T>(data);
        }
    }
}