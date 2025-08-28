using System;
using UnityEngine;

namespace Paps.ValueReferences
{
    public abstract class ValueReferenceAsset : ScriptableObject
    {
        public const string BASE_CREATE_ASSET_MENU_PATH = "Paps/Value References/";
        
        [field: SerializeField] public bool IsConstant { get; private set; }
    }
    
    public abstract class ValueReferenceAsset<T> : ValueReferenceAsset
    {
        public T Value
        {
            get => GetValue();
            set
            {
                if (IsConstant)
                    throw new InvalidOperationException("Cannot set value because it is constant");
                
                SetValue(value);
            }
        }

        protected abstract T GetValue();
        protected abstract void SetValue(T value);
    }
}
