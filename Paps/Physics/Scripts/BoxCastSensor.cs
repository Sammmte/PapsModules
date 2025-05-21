using SaintsField;
using System;
using UnityEngine;
using Paps.Optionals;
using Paps.Physics;

namespace Paps.Sensors
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

        public ReadOnlySpan<RaycastHit> Sense(OverrideParameters overrideParameters)
        {
            _overrideParameters = overrideParameters;

            var result = Sense();

            _overrideParameters = default;

            return result;
        }
        
        protected override int Execute(RaycastHit[] rayHits)
        {
            return PhysicsHelper.BoxCast(
                _overrideParameters.Origin.ValueOrDefault(Origin),
                _overrideParameters.HalfExtents.ValueOrDefault(HalfExtents),
                _overrideParameters.Direction.ValueOrDefault(Direction),
                _overrideParameters.Rotation.ValueOrDefault(Rotation),
                _overrideParameters.Distance.ValueOrDefault(Distance),
                _overrideParameters.LayerMask.ValueOrDefault(LayerMask),
                rayHits,
                _overrideParameters.QueryTriggerInteraction.ValueOrDefault(QueryTriggerInteraction)
            );
        }
    }
}