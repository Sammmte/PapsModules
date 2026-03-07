using System;
using UnityEngine;

namespace Paps.Update
{
    [Serializable]
    public struct UpdatableGroup : IEquatable<UpdatableGroup>
    {
        public const string DEFAULT_GROUP_NAME = "Default";

        public static readonly UpdatableGroup DEFAULT_GROUP = new UpdatableGroup()
        {
            Name = DEFAULT_GROUP_NAME
        };

        public static readonly UpdatableGroup NONE = default;

        public bool IsDefault => Name == DEFAULT_GROUP_NAME;

        [SerializeField] public string Name;
        public bool Equals(UpdatableGroup other)
        {
            return Name == other.Name;
        }
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}