using UnityEngine;

namespace Paps.UnityExtensions
{
    public static class RigidbodyExtensions
    {
        public static void Teleport(this Rigidbody rigidbody, Vector3 position, Quaternion rotation)
        {
            var previousInterpolation = rigidbody.interpolation;
            rigidbody.interpolation = RigidbodyInterpolation.None;
            rigidbody.position = position;
            rigidbody.rotation = rotation;
            rigidbody.interpolation = previousInterpolation;
        }
    }
}