using Paps.Optionals;
using SaintsField;
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

        public ReadOnlySpan<Collider> Sense(OverrideParameters overrideParameters)
        {
            _overrideParameters = overrideParameters;

            var result = Sense();

            _overrideParameters = default;

            return result;
        }
        
        protected override int Execute(Collider[] buffer)
        {
            return PhysicsHelper.OverlapBox(
                _overrideParameters.Origin.ValueOrDefault(Origin),
                _overrideParameters.HalfExtents.ValueOrDefault(HalfExtents),
                _overrideParameters.LayerMask.ValueOrDefault(LayerMask),
                _overrideParameters.Rotation.ValueOrDefault(Rotation),
                buffer,
                _overrideParameters.QueryTriggerInteraction.ValueOrDefault(QueryTriggerInteraction)
            );
        }
    }
}