using CareBoo.Serially;
using System;
using UnityEngine;

namespace Paps.Logging
{
    [Serializable]
    public struct LogConfigurationByType
    {
        [field: SerializeField] public SerializableType Type { get; private set; }
        [field: SerializeField] public bool Enabled { get; set; }
    }
}
