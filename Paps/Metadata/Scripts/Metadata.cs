using Cysharp.Threading.Tasks;
using Paps.Levels;
using System;
using UnityEngine;

namespace Paps.Metadata
{
    public abstract class Metadata<TKey> : MonoBehaviour, ILevelBound where TKey : struct, Enum
    {
        [field: SerializeField] public TKey Key { get; private set; }
        
        public void Loaded()
        {
            MetadataManager<TKey>.Instance.Subscribe(this);
        }

        private void OnDestroy()
        {
            MetadataManager<TKey>.Instance.Unsubscribe(this);
        }
    }
}
