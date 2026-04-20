using System;
using UnityEngine;

namespace Paps.Localization
{
    [Serializable]
    public struct LocalizationIdReference
    {
        [field: SerializeField] public TableReference TableId { get; set; }
        [field: SerializeField] public string LocalizationId { get; set; }
    }
}
