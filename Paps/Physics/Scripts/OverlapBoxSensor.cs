using Paps.Optionals;
using Paps.UnityExtensions;
using SaintsField.Playa;
using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using Cysharp.Threading.Tasks;
#endif

namespace Paps.Physics
{
    public class OverlapBoxSensor : OverlapPhysicsSensor
    {
        public struct OverrideParameters
        {
            public Optional<Vector3> Origin;
            public Optional<Vector3> HalfExtents;
            public Optional<Quaternion> Rotation;
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

        public new TempReadOnlyBufferSegment<Collider> Sense(OverrideParameters overrideParameters = default)
        {
            _overrideParameters = overrideParameters;

            var result = base.Sense();

            _overrideParameters = default;

            return result;
        }
        
        protected override int Execute(Collider[] buffer, PhysicsSensor.OverrideParameters baseFinalParameters)
        {
            var finalParameters = GetFinalParameters(baseFinalParameters, _overrideParameters);
            
            return PhysicsHelper.OverlapBox(
                finalParameters.Origin,
                finalParameters.HalfExtents,
                finalParameters.LayerMask,
                finalParameters.Rotation,
                buffer,
                finalParameters.QueryTriggerInteraction
            );
        }

        private OverrideParameters GetFinalParameters(PhysicsSensor.OverrideParameters baseFinalParameters,
            OverrideParameters overrideParameters)
        {
            return new OverrideParameters()
            {
                Origin = overrideParameters.Origin.ValueOrDefault(baseFinalParameters.Origin),
                LayerMask = overrideParameters.LayerMask.ValueOrDefault(baseFinalParameters.LayerMask),
                QueryTriggerInteraction =
                    overrideParameters.QueryTriggerInteraction.ValueOrDefault(baseFinalParameters
                        .QueryTriggerInteraction),
                HalfExtents = overrideParameters.HalfExtents.ValueOrDefault(HalfExtents),
                Rotation = overrideParameters.Rotation.ValueOrDefault(Rotation)
            };
        }

        #if UNITY_EDITOR
        private bool _onPreview;

        private void OnDrawGizmosSelected()
        {
            if(!_onPreview)
                return;

            Gizmos.DrawWireCube(Origin, HalfExtents * 2);
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