using System;
using UnityEngine;

namespace Paps.ValueReferences
{
    public abstract class ValueReferenceAsset : ScriptableObject
    {
        public const string BASE_CREATE_ASSET_MENU_PATH = "Paps/Value References/";
    }
    
    public abstract class ValueReferenceAsset<T> : ValueReferenceAsset
    {
        public T Value
        {
            get => GetValue();
        }

        protected abstract T GetValue();
    }
}
