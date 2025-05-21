using SaintsField;
using System;
using UnityEngine;

namespace Paps.Physics
{
    public abstract class CastPhysicsSensor : PhysicsSensor
    {
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

        public new ReadOnlySpan<RaycastHit> Sense()
        {
            ClearBuffer();
            
            var span = base.Sense();

            return GetLastRayHitsResults();
        }

        private int _resultCount;

        public ReadOnlySpan<RaycastHit> GetLastRayHitsResults() => new ReadOnlySpan<RaycastHit>(RayHitResults, 0, _resultCount);

        protected sealed override int Execute(Collider[] resultsBuffer)
        {
            _resultCount = Execute(RayHitResults);

            for (int i = 0; i < _resultCount; i++)
                resultsBuffer[i] = RayHitResults[i].collider;

            return _resultCount;
        }

        protected abstract int Execute(RaycastHit[] rayHits);

        private void ClearBuffer()
        {
            for (int i = 0; i < RayHitResults.Length; i++)
                RayHitResults[i] = default;
        }
    }
}