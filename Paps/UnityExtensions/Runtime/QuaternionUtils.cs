using UnityEngine;

namespace Paps.UnityExtensions
{
    public static class QuaternionUtils
    {
        public static Quaternion SmoothDamp(Quaternion current, Quaternion target, ref Vector3 currentVelocity, 
            float smoothTime, float maxSpeed, float deltaTime)
        {
            if(deltaTime == 0)
            {
                currentVelocity = Vector3.zero;
                return current;
            }

            if(smoothTime == 0)
            {
                currentVelocity = Vector3.zero;
                return target;
            }

            Vector3 c = current.eulerAngles;
            Vector3 t = target.eulerAngles;
            return Quaternion.Euler(
                Mathf.SmoothDampAngle(c.x, t.x, ref currentVelocity.x, smoothTime, maxSpeed, deltaTime),
                Mathf.SmoothDampAngle(c.y, t.y, ref currentVelocity.y, smoothTime, maxSpeed, deltaTime),
                Mathf.SmoothDampAngle(c.z, t.z, ref currentVelocity.z, smoothTime, maxSpeed, deltaTime)
            );
        }
    }
}