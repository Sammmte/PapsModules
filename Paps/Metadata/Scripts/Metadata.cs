using Cysharp.Threading.Tasks;
using Paps.LevelSetup;
using System;
using UnityEngine;

namespace Paps.Metadata
{
    public abstract class Metadata<TKey> : MonoBehaviour, ILevelSetuppable where TKey : struct, Enum
    {
        [field: SerializeField] public TKey Key { get; private set; }
        
        private void Awake()
        {
            MetadataManager<TKey>.Instance.Subscribe(this);
        }

        public async UniTask Setup()
        {
            MetadataManager<TKey>.Instance.Subscribe(this);
        }

        public async UniTask Unload()
        {
            MetadataManager<TKey>.Instance.Unsubscribe(this);
        }

        private void OnDestroy()
        {
            MetadataManager<TKey>.Instance.Unsubscribe(this);
        }
    }

    public abstract class Metadata<TKey, TValue> : Metadata<TKey> where TKey : struct, Enum
    {
        public abstract TValue Value
        {
            get;
        }
    }
}
