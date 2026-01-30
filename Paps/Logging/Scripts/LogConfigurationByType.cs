using SaintsField;
using System;
using UnityEngine;

namespace Paps.Logging
{
    [Serializable]
    public struct LogConfigurationByType
    {
        [field: SerializeField, TypeReference(EType.AllAssembly)]
        public TypeReference Type { get; private set; }
        [field: SerializeField] public bool Enabled { get; set; }
    }
}
