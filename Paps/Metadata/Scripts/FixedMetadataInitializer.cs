using Paps.Levels;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Paps.Metadata
{
    public abstract class FixedMetadataInitializer<TKey> : MonoBehaviour, ILevelBound where TKey : struct, Enum
    {
        private List<TKey> _addedKeys;

        public void Loaded()
        {
            _addedKeys = new List<TKey>(MetadataManager<TKey>.KEYS_AMOUNT);
            OnInitializeMetadata();
        }

        private void OnDestroy()
        {
            for(int i = 0; i < _addedKeys.Count; i++)
            {
                MetadataManager<TKey>.Instance.Remove(gameObject, _addedKeys[i]);
            }
        }

        protected abstract void OnInitializeMetadata();

        protected void SubscribeMetadata<T>(TKey key, T value)
        {
            MetadataManager<TKey>.Instance.Add(gameObject, key, value);
            _addedKeys.Add(key);
        }
    }
}
