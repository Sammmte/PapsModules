using UnityEngine;
using Paps.Optionals;
#if UNITY_EDITOR
using SaintsField.Playa;
using UnityEditor;
using Cysharp.Threading.Tasks;
#endif


namespace Paps.Physics
{
    public class SphereCastSensor : CastPhysicsSensor
    {
        new public struct OverrideParameters
        {
            public Optional<Vector3> Origin;
            public Optional<Vector3> Direction;
            public Optional<float> Distance;
            public Optional<LayerMask> LayerMask;
            public Optional<QueryTriggerInteraction> QueryTriggerInteraction;
            public Optional<bool> OrderByDistance;
            public Optional<float> Radius;
        }

        [field: SerializeField] public float Radius { get; private set; }

        private OverrideParameters _overrideParameters;

        protected override int Execute(RaycastHit[] rayHits, CastPhysicsSensor.OverrideParameters baseFinalParameters)
        {
            var finalParameters = GetFinalParameters(baseFinalParameters, _overrideParameters);
            
            if (MaxResults == 1)
            {
                if (PhysicsHelper.SphereCast(
                        finalParameters.Origin,
                        finalParameters.Direction,
                        finalParameters.Radius,
                        out RaycastHit hitInfo,
                        finalParameters.Distance,
                        finalParameters.LayerMask,
                        finalParameters.QueryTriggerInteraction
                    ))
                {
                    rayHits[0] = hitInfo;
                    return 1;
                }

                return 0;
            }
            
            return PhysicsHelper.SphereCast(
                finalParameters.Origin,
                finalParameters.Direction,
                finalParameters.Radius,
                rayHits,
                finalParameters.Distance,
                finalParameters.LayerMask,
                finalParameters.QueryTriggerInteraction,
                finalParameters.OrderByDistance
            );
        }

        private OverrideParameters GetFinalParameters(CastPhysicsSensor.OverrideParameters baseFinalParameters,
            OverrideParameters overrideParameters)
        {
            return new OverrideParameters()
            {
                Origin = overrideParameters.Origin.ValueOrDefault(baseFinalParameters.Origin),
                LayerMask = overrideParameters.LayerMask.ValueOrDefault(baseFinalParameters.LayerMask),
                QueryTriggerInteraction =
                    overrideParameters.QueryTriggerInteraction.ValueOrDefault(baseFinalParameters
                        .QueryTriggerInteraction),
                Direction = overrideParameters.Direction.ValueOrDefault(baseFinalParameters.Direction),
                Distance = overrideParameters.Distance.ValueOrDefault(baseFinalParameters.Distance),
                OrderByDistance = overrideParameters.OrderByDistance.ValueOrDefault(baseFinalParameters.OrderByDistance),
                Radius = overrideParameters.Radius.ValueOrDefault(Radius)
            };
        }

        #if UNITY_EDITOR
        private bool _onPreview;
        private Vector3 _nextPreviewPosition;

        private void OnDrawGizmosSelected()
        {
            if(!_onPreview)
                return;

            Gizmos.DrawWireSphere(_nextPreviewPosition, Radius);
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
            var origin = Origin;
            var endPosition = origin + (Direction.normalized * Distance);

            var lastRecordedTime = EditorApplication.timeSinceStartup;
            var deltaTime = 0f;
            var currentTime = 0f;

            _onPreview = true;

            while(currentTime < totalSeconds)
            {
                var nextPosition = Vector3.Lerp(origin, endPosition, currentTime / totalSeconds);

                _nextPreviewPosition = nextPosition;

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