using Nomnom.RaycastVisualization;
using UnityEngine;
#if UNITY_EDITOR
using PhysicsAPI = Nomnom.RaycastVisualization.VisualPhysics;
#else
using PhysicsAPI = UnityEngine.Physics;
#endif

namespace Paps.Physics
{
    public static class PhysicsHelper
    {
        public static float EDITOR_GIZMO_DISPLAY_DURATION = 2f;

        private static readonly Collider[] _simpleColliderBuffer = new Collider[1];
        private static readonly RaycastHit[] _simpleRaycastHitBuffer = new RaycastHit[50];

        public static int Raycast(Vector3 origin, Vector3 direction, float maxDistance, LayerMask layerMask,
            RaycastHit[] rayHits, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal, 
            bool orderByDistance = false)
        {
            using (VisualLifetime.Create(EDITOR_GIZMO_DISPLAY_DURATION))
            {
                var count = PhysicsAPI.RaycastNonAlloc(origin, direction, rayHits, maxDistance, layerMask,
                    queryTriggerInteraction);

                if (orderByDistance)
                    OrderHitsByDistance(rayHits, count);
                
                return count;
            }
        }

        private static void OrderHitsByDistance(RaycastHit[] buffer, int count)
        {
            for (int i = 0; i < count; i++)
            {
                for (int j = i + 1; j < count; j++)
                {
                    if (buffer[i].distance > buffer[j].distance)
                    {
                        var temp = buffer[i];
                        buffer[i] = buffer[j];
                        buffer[j] = temp;
                    }
                }
            }
        }
        
        public static bool Raycast(Vector3 origin, Vector3 direction, float maxDistance, LayerMask layerMask,
            out RaycastHit hitInfo, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
        {
            var amount = Raycast(origin, direction, maxDistance, layerMask, _simpleRaycastHitBuffer,
                queryTriggerInteraction);
            
            GetRaycastHitFromBuffer(amount, out hitInfo);

            return amount > 0;
        }

        public static int OverlapBox(Vector3 center, Vector3 halfExtents, LayerMask layerMask, Quaternion rotation,
            Collider[] buffer, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
        {
            using (VisualLifetime.Create(EDITOR_GIZMO_DISPLAY_DURATION))
            {
                return PhysicsAPI.OverlapBoxNonAlloc(center, halfExtents, buffer, rotation, layerMask, queryTriggerInteraction);
            }
        }

        public static bool OverlapBox(Vector3 center, Vector3 halfExtents, LayerMask layerMask, Quaternion rotation, out Collider collider, 
            QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
        {
            OverlapBox(center, halfExtents, layerMask, rotation, _simpleColliderBuffer, queryTriggerInteraction);

            return GetColliderFromBuffer(out collider);
        }

        private static bool GetColliderFromBuffer(out Collider collider)
        {
            collider = _simpleColliderBuffer[0];

            _simpleColliderBuffer[0] = null;
            
            return collider != null;
        }

        private static void GetRaycastHitFromBuffer(int amount, out RaycastHit raycastHit)
        {
            var closest = _simpleRaycastHitBuffer[0];
            
            for (int i = 1; i < amount; i++)
            {
                var current = _simpleRaycastHitBuffer[i];

                if (current.distance < closest.distance)
                    closest = current;

                _simpleRaycastHitBuffer[i] = default;
            }

            raycastHit = closest;
        }

        public static bool BoxCast(Vector3 center, Vector3 halfExtents, Vector3 direction, Quaternion rotation, float maxDistance,
            LayerMask layerMask, out RaycastHit hitInfo, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
        {
            var amount = BoxCast(center, halfExtents, direction, rotation, maxDistance, layerMask,
                _simpleRaycastHitBuffer, queryTriggerInteraction);
            
            GetRaycastHitFromBuffer(amount, out hitInfo);

            return amount > 0;
        }
        
        public static int BoxCast(Vector3 center, Vector3 halfExtents, Vector3 direction, Quaternion rotation, float maxDistance,
            LayerMask layerMask, RaycastHit[] hitInfo, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
        {
            using (VisualLifetime.Create(EDITOR_GIZMO_DISPLAY_DURATION))
            {
                return PhysicsAPI.BoxCastNonAlloc(center, halfExtents, direction, hitInfo, rotation, maxDistance,
                    layerMask, queryTriggerInteraction);
            }
        }
    }
}
