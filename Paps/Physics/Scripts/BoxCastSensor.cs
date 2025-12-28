using UnityEngine;
using Paps.Optionals;
using Paps.UnityExtensions;
using SaintsField.Playa;
#if UNITY_EDITOR
using UnityEditor;
using Cysharp.Threading.Tasks;
#endif

namespace Paps.Physics
{
    public class BoxCastSensor : CastPhysicsSensor
    {
        public struct OverrideParameters
        {
            public Optional<Vector3> Origin;
            public Optional<Vector3> Direction;
            public Optional<Vector3> HalfExtents;
            public Optional<Quaternion> Rotation;
            public Optional<float> Distance;
            public Optional<LayerMask> LayerMask;
            public Optional<QueryTriggerInteraction> QueryTriggerInteraction;
        }

        [SerializeField] private bool _useTransformForRotation;

        [SerializeField] [ShowIf(nameof(_useTransformForRotation))] private Transform _rotationSource;

        [SerializeField] [HideIf(nameof(_useTransformForRotation))] private Space _rotationSpace;
        [SerializeField] [HideIf(nameof(_useTransformForRotation))] private Vector3 _rotation;
        [field: SerializeField] public Vector3 HalfExtents { get; private set; }

        public Quaternion Rotation
        {
            get
            {
                if (_useTransformForRotation)
                    return _rotationSource.rotation;

                if (_rotationSpace == Space.World)
                    return Quaternion.Euler(_rotation);
                else
                    return transform.rotation * Quaternion.Euler(_rotation);
            }
        }

        private OverrideParameters _overrideParameters;

        public new TempReadOnlyBufferSegment<RaycastHit> Sense(OverrideParameters overrideParameters = default)
        {
            _overrideParameters = overrideParameters;

            var result = base.Sense();

            _overrideParameters = default;

            return result;
        }
        
        protected override int Execute(RaycastHit[] rayHits, CastPhysicsSensor.OverrideParameters baseFinalParameters)
        {
            var finalParameters = GetFinalParameters(baseFinalParameters, _overrideParameters);
            
            if (MaxResults == 1)
            {
                if (PhysicsHelper.BoxCast(
                        finalParameters.Origin,
                        finalParameters.HalfExtents,
                        finalParameters.Direction,
                        finalParameters.Rotation,
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
            
            return PhysicsHelper.BoxCast(
                finalParameters.Origin,
                finalParameters.HalfExtents,
                finalParameters.Direction,
                finalParameters.Rotation,
                finalParameters.Distance,
                finalParameters.LayerMask,
                rayHits,
                finalParameters.QueryTriggerInteraction
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
                HalfExtents = overrideParameters.HalfExtents.ValueOrDefault(HalfExtents),
                Rotation = overrideParameters.Rotation.ValueOrDefault(Rotation)
            };
        }

        #if UNITY_EDITOR
        private bool _onPreview;
        private Vector3 _nextPreviewPosition;

        private void OnDrawGizmosSelected()
        {
            if(!_onPreview)
                return;

            Gizmos.DrawWireCube(_nextPreviewPosition, HalfExtents * 2);
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