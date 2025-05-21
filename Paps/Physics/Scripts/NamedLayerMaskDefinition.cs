using System;
using UnityEngine;

namespace Paps.Physics
{
    [Serializable]
    internal struct NamedLayerMaskDefinition
    {
        [SerializeField] public string Name;
        [SerializeField] public LayerMask LayerMask;

        public static implicit operator NamedLayerMask(NamedLayerMaskDefinition definition)
        {
            return new NamedLayerMask() { Name = definition.Name };
        }
    }
}