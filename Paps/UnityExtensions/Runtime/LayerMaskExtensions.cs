using UnityEngine;

namespace Paps.UnityExtensions
{
    public static class LayerMaskExtensions
    {
        public static bool Includes(this int value, params int[] layers)
        {
            foreach(var layer in layers)
            {
                if ((value & 1 << layer) <= 0)
                    return false;
            }

            return true;
        }

        public static bool Includes(this LayerMask mask, params int[] layers)
        {
            return mask.value.Includes(layers);
        }
    }
}