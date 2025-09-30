using System;
using UnityEngine;

namespace Paps.Metadata
{
    public static class ComponentExtensions
    {
        public static bool TryGetMetadata<TKey, TValue>(this GameObject gameObject, TKey propertyKey,
            out TValue result) where TKey : struct, Enum
        {
            return MetadataManager<TKey>.Instance.TryGetMetadata(gameObject, propertyKey, out result);
        }
        
        public static bool TryGetMetadata<TKey, TValue>(this Component component, TKey propertyKey,
            out TValue result) where TKey : struct, Enum
        {
            return MetadataManager<TKey>.Instance.TryGetMetadata(component.gameObject, propertyKey, out result);
        }
    }
}