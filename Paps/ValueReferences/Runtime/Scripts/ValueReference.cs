using Paps.Optionals;
using SaintsField;
using SaintsField.Playa;
using System;
using UnityEngine;

namespace Paps.ValueReferences
{
    [Serializable]
    public struct ValueReference<T>
    {
        [SerializeField] private SaintsInterface<IValueReferenceSource<T>> _referenceSource;

        [SerializeField] private Optional<T> _hardcodedValue;
        [SerializeField] private bool _throwErrorWhenNoValuePresent;

        [ShowInInspector]
        public T Value
        {
            get
            {
                if(_referenceSource.I != null)
                    return _referenceSource.I.Value;

                if (_hardcodedValue.HasValue)
                    return _hardcodedValue.Value;

                if (_throwErrorWhenNoValuePresent)
                    throw new InvalidOperationException("Value not present");

                return default;
            }
        }

        public bool HasValue => _referenceSource.I != null || _hardcodedValue.HasValue;
        
        public static implicit operator T(ValueReference<T> valueReference) => valueReference.Value;
    }
}