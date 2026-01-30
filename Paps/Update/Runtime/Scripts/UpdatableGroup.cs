using System;
using UnityEngine;

namespace Paps.Update
{
    [Serializable]
    public struct UpdatableGroup : IEquatable<UpdatableGroup>
    {
        public const string DEFAULT_GROUP_NAME = "Default";
        public const int DEFAULT_GROUP_ID = 0;

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