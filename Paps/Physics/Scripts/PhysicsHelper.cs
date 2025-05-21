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
        
        public static bool Raycast(Vector3 origin, Vector3 direction, float maxDistance, int layerMask,
            out RaycastHit hitInfo, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
        {
            using (VisualLifetime.Create(EDITOR_GIZMO_DISPLAY_DURATION))
            {
                return PhysicsAPI.Raycast(origin, direction, out hitInfo, maxDistance, layerMask, queryTriggerInteraction);
            }
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

        public static bool SphereCast(Vector3 origin, float radius, Vector3 direction, out RaycastHit hitInfo, 
            float maxDistance, LayerMask layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
        {
            using (VisualLifetime.Create(EDITOR_GIZMO_DISPLAY_DURATION))
            {
                return PhysicsAPI.SphereCast(origin, radius, direction, out hitInfo, maxDistance, layerMask, queryTriggerInteraction);
            }
        }

        public static bool BoxCast(Vector3 center, Vector3 halfExtents, Vector3 direction, Quaternion rotation, float maxDistance,
            LayerMask layerMask, out RaycastHit hitInfo, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
        {
            using (VisualLifetime.Create(EDITOR_GIZMO_DISPLAY_DURATION))
            {
                return PhysicsAPI.BoxCast(center, halfExtents, direction, out hitInfo, rotation, maxDistance, layerMask, queryTriggerInteraction);
            }
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
