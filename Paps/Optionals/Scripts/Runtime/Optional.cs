using System;
using UnityEngine;

namespace Paps.Optionals
{
    [Serializable]
    public struct Optional<T> : ISerializationCallbackReceiver
    {
        [SerializeField] private bool _considerItHasValue;
        [SerializeField] private T _value;

        public T Value
        {
            get
            {
                if (!HasValue)
                    throw new InvalidOperationException("Optional has no value");
                
                return _value;
            }
        }

        public bool HasValue { get; private set; }

        public Optional(T value)
        {
            _value = value;
            
            HasValue = IsValidValue(value);
            _considerItHasValue = HasValue;
        }

        public Optional(T value, bool hasValue)
        {
            _value = value;
            
            if (hasValue)
                HasValue = IsValidValue(_value);
            else
                HasValue = false;

            _considerItHasValue = HasValue;
        }

        private static bool IsValidValue(T value)
        {
            var type = typeof(T);

            if (type.IsClass)
            {
                if (value is UnityEngine.Object unityObject)
                    return unityObject != null;

                return value != null;
            }
            
            return true;
        }

        public static implicit operator T(Optional<T> optional)
        {
            return optional.Value;
        }

        public static implicit operator Optional<T>(T value)
        {
            return new Optional<T>(value);
        }
        
        public static Optional<T> None() => new Optional<T>();

        public void OnBeforeSerialize()
        {
            
        }

        public void OnAfterDeserialize()
        {
            if(_considerItHasValue)
            {
                HasValue = IsValidValue(_value);
            }
            else
            {
                HasValue = false;
            }
        }
    }
}
