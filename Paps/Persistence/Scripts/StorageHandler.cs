using Cysharp.Threading.Tasks;
using Paps.Logging;
using Unity.Serialization.Json;
using UnityEngine;

namespace Paps.Persistence
{
    public class StorageHandler
    {
        public static bool PersistenceEnabled { get; set; }

        private InMemoryStorage _inMemoryStorage = new InMemoryStorage();
        private IStorage _actualStorage;
        private ISerializer _serializer;

        public bool HasData => GetStorage().HasData;

        public StorageHandler(IStorage storage, ISerializer serializer)
        {
            _actualStorage = storage;
            _serializer = serializer;
        }

        public async UniTask<T> LoadAsync<T>(T defaultValue = default)
        {
            if (!HasData)
                return defaultValue;

            var serializedData = await GetStorage().LoadAsync();
            this.Log($"Loading data:\n\n{serializedData}");
            return _serializer.Deserialize<T>(serializedData);
        }

        public async UniTask SaveAsync<T>(T data)
        {
            var serializedData = _serializer.Serialize(data);
            this.Log($"Saving data:\n\n{serializedData}");
            await GetStorage().SaveAsync(serializedData);
        }

        private IStorage GetStorage()
        {
#if PERSISTENCE || UNITY_EDITOR
            if (PersistenceEnabled)
                return _actualStorage;
            else
                return _inMemoryStorage;
#else
            return _inMemoryStorage;
#endif
        }
    }
}