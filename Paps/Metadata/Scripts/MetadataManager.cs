using SaintsField.Playa;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Paps.Metadata
{
    public abstract class MetadataManager<TKey> : MonoBehaviour where TKey : struct, Enum
    {
        public static MetadataManager<TKey> Instance { get; private set; }

        public static readonly int KEYS_AMOUNT = Enum.GetValues(typeof(TKey)).Length;

        [SerializeField] private int _gameObjectsCapacity;

        private ObjectPool<Dictionary<TKey, Metadata>> _metadataDictionaryPool;

        [ShowInInspector] private Dictionary<GameObject, Dictionary<TKey, Metadata>> _describedGameObjects;

        private void Awake()
        {
            _describedGameObjects = new Dictionary<GameObject, Dictionary<TKey, Metadata>>(_gameObjectsCapacity);
            _metadataDictionaryPool = new ObjectPool<Dictionary<TKey, Metadata>>(CreateDictionary, defaultCapacity: _gameObjectsCapacity);

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private Dictionary<TKey, Metadata> CreateDictionary() => new Dictionary<TKey, Metadata>(KEYS_AMOUNT);

        public void Add<T>(GameObject gameObject, TKey key, T data)
        {
            if (!_describedGameObjects.ContainsKey(gameObject))
            {
                _describedGameObjects[gameObject] = _metadataDictionaryPool.Get();
            }

            if(_describedGameObjects[gameObject].ContainsKey(key))
                throw new InvalidOperationException($"Metadata key {key} already exists");

            _describedGameObjects[gameObject].Add(key, Metadata<T>.GetPooled(data, KEYS_AMOUNT));
        }

        public void Remove(GameObject gameObject, TKey key)
        {
            if(!_describedGameObjects.ContainsKey(gameObject))
                return;

            var dictionary = _describedGameObjects[gameObject];

            if(dictionary.Remove(key, out var metadata))
            {
                metadata.Release();
            }

            if(dictionary.Count == 0)
            {
                _describedGameObjects.Remove(gameObject);
                _metadataDictionaryPool.Release(dictionary);
            }
        }

        public bool TryGetMetadata<TValue>(GameObject go, TKey propertyKey, out TValue result)
        {
            if (_describedGameObjects.TryGetValue(go, out var keyDictionary))
            {
                if (keyDictionary.TryGetValue(propertyKey, out var metadata) &&
                    metadata is Metadata<TValue> specialized)
                {
                    result = specialized.Value;
                    return true;
                }
            }

            result = default;
            return false;
        }
    }
}