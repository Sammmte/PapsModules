using System;
using UnityEngine;

namespace Paps.Optionals
{
    [Serializable]
    public struct Optional<T>
    {
        [SerializeField] private bool _serializedByUnityFlag;
        [SerializeField] private bool _considerItHasValue;
        [SerializeField] private T _value;

        private bool _hasValue;

        public T Value
        {
            get
            {
                if (!HasValue)
                    throw new InvalidOperationException("Optional has no value");
                
                return _value;
            }
        }

        public bool HasValue
        {
            get
            {
                if (_serializedByUnityFlag && !_considerItHasValue)
                {
                    _hasValue = false;
                    _serializedByUnityFlag = false;
                }
                else if (_serializedByUnityFlag && _considerItHasValue)
                {
                    _hasValue = IsValidValue(_value);
                    _serializedByUnityFlag = false;
                }

                return _hasValue;
            }
        }

        public Optional(T value)
        {
            _value = value;
            _considerItHasValue = false;
            _serializedByUnityFlag = false;

            _hasValue = IsValidValue(value);
        }

        public Optional(T value, bool hasValue)
        {
            _value = value;
            _serializedByUnityFlag = false;
            _considerItHasValue = false;
            
            if (hasValue)
                _hasValue = IsValidValue(_value);
            else
                _hasValue = false;
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
    }
}
