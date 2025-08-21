using UnityEngine;

namespace Paps.UnityExtensions
{
    public static class LayerMaskExtensions
    {
        public static bool Includes(this LayerMask mask, int layer)
        {
            return (mask.value & (1 << layer)) != 0;
        }

        public static bool Includes(this LayerMask mask, GameObject gameObject) => 
            Includes(mask, gameObject.layer);
        public static bool Includes(this LayerMask mask, Component component) =>
            Includes(mask, component.gameObject.layer);
    }
}