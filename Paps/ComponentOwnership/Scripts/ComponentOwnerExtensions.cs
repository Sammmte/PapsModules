using UnityEngine;

namespace Paps.ComponentOwnership
{
    public static class ComponentOwnerExtensions
    {
        public static bool TryGetOwner<T>(this Component component, out T owner)
        {
            return ComponentOwnershipManager.Instance.TryGetOwner(component, out owner);
        }
    }
}