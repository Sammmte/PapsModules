using System;
using UnityEngine;

namespace Paps.Localization
{
    [Serializable]
    public struct TableReference
    {
        [field: SerializeField] public string TableId { get; set; }

        public static implicit operator string(TableReference reference) => reference.TableId;
        public static implicit operator TableReference(string tableId) => new TableReference() { TableId = tableId };
    }
}
