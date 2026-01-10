using Cysharp.Threading.Tasks;
using Paps.LevelSetup;
using SaintsField.Playa;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Paps.Metadata
{
    public abstract class MetadataManager<TKey> : MonoBehaviour, ILevelSetuppable where TKey : struct, Enum
    {
        public static MetadataManager<TKey> Instance { get; private set; }

        [SerializeField] private int _gameObjectsCapacity;

        private int _keysAmount;

        [ShowInInspector] private Dictionary<GameObject, Dictionary<TKey, Metadata<TKey>>> _describedGameObjects;

        private void Awake()
        {
            _describedGameObjects = new Dictionary<GameObject, Dictionary<TKey, Metadata<TKey>>>(_gameObjectsCapacity);
            _keysAmount = Enum.GetValues(typeof(TKey)).Length;

            Instance = this;
            DontDestroyOnLoad(gameObject);
            LevelSetupper.Instance.RegisterEverPresent(this);
        }

        public void Subscribe(Metadata<TKey> metadata)
        {
            if (!_describedGameObjects.ContainsKey(metadata.gameObject))
            {
                _describedGameObjects[metadata.gameObject] =
                    new Dictionary<TKey, Metadata<TKey>>(_keysAmount);
            }
            
            _describedGameObjects[metadata.gameObject][metadata.Key] = metadata;
        }

        public void Unsubscribe(Metadata<TKey> metadata)
        {
            if (_describedGameObjects.ContainsKey(metadata.gameObject))
            {
                _describedGameObjects[metadata.gameObject].Remove(metadata.Key);
                if (_describedGameObjects[metadata.gameObject].Count == 0)
                {
                    _describedGameObjects.Remove(metadata.gameObject);
                }
            }
        }

        public bool TryGetMetadata<TValue>(GameObject go, TKey propertyKey, out TValue result)
        {
            if (_describedGameObjects.TryGetValue(go, out var keyDictionary))
            {
                if (keyDictionary.TryGetValue(propertyKey, out var metadata) &&
                    metadata is IMetadata<TValue> specialized)
                {
                    result = specialized.Value;
                    return true;
                }
            }

            result = default;
            return false;
        }

        public async UniTask Unload()
        {
            _describedGameObjects.Clear();
        }
    }
}