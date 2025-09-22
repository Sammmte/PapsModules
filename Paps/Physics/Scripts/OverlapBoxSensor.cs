using Paps.Optionals;
using SaintsField.Playa;
using System;
using UnityEngine;

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

        public new ReadOnlySpan<Collider> Sense(OverrideParameters overrideParameters = default)
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
    }
}