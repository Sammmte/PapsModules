using Paps.Optionals;
using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using Cysharp.Threading.Tasks;
using SaintsField.Playa;
#endif

namespace Paps.Physics
{
    public class RaycastSensor : CastPhysicsSensor
    {
        protected override int Execute(RaycastHit[] rayHits, CastPhysicsSensor.OverrideParameters finalParameters)
        {
            if (MaxResults == 1)
            {
                if (PhysicsHelper.Raycast(
                        finalParameters.Origin,
                        finalParameters.Direction,
                        finalParameters.Distance,
                        finalParameters.LayerMask,
                        out RaycastHit hitInfo,
                        finalParameters.QueryTriggerInteraction
                    ))
                {
                    rayHits[0] = hitInfo;
                    return 1;
                }

                return 0;
            }

            return PhysicsHelper.Raycast(
                finalParameters.Origin,
                finalParameters.Direction,
                finalParameters.Distance,
                finalParameters.LayerMask,
                rayHits,
                finalParameters.QueryTriggerInteraction,
                finalParameters.OrderByDistance
            );
        }

        #if UNITY_EDITOR
        private bool _onPreview;

        private void OnDrawGizmosSelected()
        {
            if(!_onPreview)
                return;

            Gizmos.color = Color.green;
            Gizmos.DrawLine(Origin, Direction * Distance);
        }

        [Button]
        private void Preview(float totalSeconds = 1f)
        {
            if(totalSeconds <= 0)
            {
                totalSeconds = 0.001f;
            }

            ShowPreview(totalSeconds).Forget();
        }

        private async UniTaskVoid ShowPreview(float totalSeconds)
        {
            var lastRecordedTime = EditorApplication.timeSinceStartup;
            var deltaTime = 0f;
            var currentTime = 0f;

            _onPreview = true;

            while(currentTime < totalSeconds)
            {
                deltaTime = (float)(EditorApplication.timeSinceStartup - lastRecordedTime);
                lastRecordedTime = EditorApplication.timeSinceStartup;

                currentTime += deltaTime;

                await UniTask.NextFrame();
            }

            _onPreview = false;
        }
        #endif
    }
}