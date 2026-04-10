using SaintsField;
using System.Collections.Generic;
using Unity.Serialization.Json;
using UnityEngine;

namespace Paps.Persistence
{
    public class UnitySerializationJsonSerializer : ScriptableObject, ISerializer
    {
        [SerializeField] private SaintsInterface<IJsonAdapter>[] _jsonAdapters;

        private List<IJsonAdapter> _preparedAdapters;

        public T Deserialize<T>(string data)
        {
            EnsureAdaptersPrepared();

            return JsonSerialization.FromJson<T>(data, new JsonSerializationParameters()
            {
                UserDefinedAdapters = _preparedAdapters
            });
        }

        public void Deserialize<T>(string data, ref T container)
        {
            EnsureAdaptersPrepared();

            JsonSerialization.FromJsonOverride(data, ref container, new JsonSerializationParameters()
            {
                UserDefinedAdapters = _preparedAdapters
            });
        }

        public string Serialize<T>(T obj)
        {
            EnsureAdaptersPrepared();

            return JsonSerialization.ToJson(obj, new JsonSerializationParameters()
            {
                Minified =
                #if PRODUCTION
                true
                #else
                false
                #endif
                ,
                UserDefinedAdapters = _preparedAdapters
            });
        }

        private void EnsureAdaptersPrepared()
        {
            if(_preparedAdapters == null)
            {
                _preparedAdapters = new List<IJsonAdapter>(_jsonAdapters.Length);

                for(int i = 0; i < _jsonAdapters.Length; i++)
                {
                    _preparedAdapters.Add(_jsonAdapters[i].I);
                }
            }
        }
    }
}