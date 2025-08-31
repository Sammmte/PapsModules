using System;
using UnityEngine;

namespace Paps.ValueReferences
{
    [Serializable]
    public class ValueReferenceGroup
    {
        [field: SerializeField] public string GroupPath { get; set; }
        [field: SerializeField] public ValueReferenceAsset[] ValueReferences { get; set; }
        [field: SerializeField] public ValueReferenceGroup[] SubGroups { get; set; }
    }
}