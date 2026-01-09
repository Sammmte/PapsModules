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

        [ShowInInspector]
        public T Value
        {
            get
            {
                if(_referenceSource.I != null)
                    return _referenceSource.I.Value;

                if (_hardcodedValue.HasValue)
                    return _hardcodedValue.Value;

                throw new InvalidOperationException("Value not present");
            }
        }

        public bool HasValue => _referenceSource.I != null || _hardcodedValue.HasValue;
        
        public static implicit operator T(ValueReference<T> valueReference) => valueReference.Value;
    }
}