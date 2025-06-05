using Paps.Optionals;
using SaintsField;
using System;
using UnityEngine;

namespace Paps.Physics
{
    public abstract class CastPhysicsSensor : PhysicsSensor
    {
        public struct OverrideParameters
        {
            public Optional<Vector3> Origin;
            public Optional<LayerMask> LayerMask;
            public Optional<QueryTriggerInteraction> QueryTriggerInteraction;
            public Optional<Vector3> Direction;
            public Optional<float> Distance;
        }
        
        [SerializeField] private bool _useTransformForDirection;
        [SerializeField] [ShowIf(nameof(_useTransformForDirection))] private Transform _forwardDirectionTransform;

        [SerializeField] [HideIf(nameof(_useTransformForDirection))] private Vector3 _direction;
        [field: SerializeField] public float Distance { get; private set; }

        public Vector3 Direction
        {
            get
            {
                if (_useTransformForDirection)
                    return _forwardDirectionTransform.forward;

                return _direction.normalized;
            }
        }

        private RaycastHit[] _rayHitResults;

        private RaycastHit[] RayHitResults
        {
            get
            {
                if (_rayHitResults == null)
                    _rayHitResults = new RaycastHit[MaxResults];

                return _rayHitResults;
            }
        }

        private OverrideParameters _overrideParameters;

        public new ReadOnlySpan<RaycastHit> Sense(OverrideParameters overrideParameters = default)
        {
            ClearBuffer();
            _overrideParameters = overrideParameters;
            
            var span = base.Sense();

            _overrideParameters = default;

            return GetLastRayHitsResults();
        }

        private int _rayResultCount;

        public ReadOnlySpan<RaycastHit> GetLastRayHitsResults() => new ReadOnlySpan<RaycastHit>(RayHitResults, 0, _rayResultCount);

        protected sealed override int Execute(Collider[] resultsBuffer, PhysicsSensor.OverrideParameters baseFinalParameters)
        {
            _rayResultCount = Execute(RayHitResults, GetFinalParameters(baseFinalParameters, _overrideParameters));

            for (int i = 0; i < _rayResultCount; i++)
                resultsBuffer[i] = RayHitResults[i].collider;

            return _rayResultCount;
        }

        protected abstract int Execute(RaycastHit[] rayHits, OverrideParameters finalParameters);

        private void ClearBuffer()
        {
            for (int i = 0; i < RayHitResults.Length; i++)
                RayHitResults[i] = default;
        }

        private OverrideParameters GetFinalParameters(PhysicsSensor.OverrideParameters baseFinalParameters,
            OverrideParameters overrideParameters)
        {
            return new OverrideParameters()
            {
                Origin = overrideParameters.Origin.ValueOrDefault(baseFinalParameters.Origin),
                LayerMask = overrideParameters.LayerMask.ValueOrDefault(baseFinalParameters.LayerMask),
                QueryTriggerInteraction = overrideParameters.QueryTriggerInteraction.ValueOrDefault(
                    baseFinalParameters.QueryTriggerInteraction),
                Direction = overrideParameters.Direction.ValueOrDefault(Direction),
                Distance = overrideParameters.Distance.ValueOrDefault(Distance)
            };
        }
    }
}