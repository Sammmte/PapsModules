using Paps.Optionals;
using SaintsField;
using System;
using UnityEngine;

namespace Paps.Physics
{
    public abstract class PhysicsSensor : MonoBehaviour
    {
        public struct OverrideParameters
        {
            public Optional<Vector3> Origin;
            public Optional<LayerMask> LayerMask;
            public Optional<QueryTriggerInteraction> QueryTriggerInteraction;
        }
        
        [field: SerializeField] [field: Min(1)] public int MaxResults { get; private set; }
        [SerializeField] private bool _useNamedLayerMask;
        [SerializeField] [ShowIf(nameof(_useNamedLayerMask))] private NamedLayerMask _namedLayerMask;
        [SerializeField] [HideIf(nameof(_useNamedLayerMask))] private LayerMask _layerMask;
        [field: SerializeField] public QueryTriggerInteraction QueryTriggerInteraction { get; private set; }
        [SerializeField] private Transform _origin;
        
        public Vector3 Origin => _origin.position;

        public LayerMask LayerMask
        {
            get
            {
                if (_useNamedLayerMask)
                    return _namedLayerMask.LayerMask;

                return _layerMask;
            }
        }
        
        private Collider[] _collidersBuffer;

        private Collider[] CollidersBuffer
        {
            get
            {
                if (_collidersBuffer == null)
                    _collidersBuffer = new Collider[MaxResults];

                return _collidersBuffer;
            }
        }

        private int _resultCount;

        public ReadOnlySpan<Collider> Sense(OverrideParameters overrideParameters = default)
        {
            ClearBuffer();
            
            _resultCount = Execute(CollidersBuffer, GetFinalParameters(overrideParameters));

            return GetLastResults();
        }

        public ReadOnlySpan<Collider> GetLastResults() => new ReadOnlySpan<Collider>(CollidersBuffer, 0, _resultCount);

        protected abstract int Execute(Collider[] resultsBuffer, OverrideParameters finalParameters);

        private void ClearBuffer()
        {
            for (int i = 0; i < CollidersBuffer.Length; i++)
                CollidersBuffer[i] = null;
        }

        private OverrideParameters GetFinalParameters(OverrideParameters inputParameters)
        {
            return new OverrideParameters()
            {
                Origin = inputParameters.Origin.ValueOrDefault(Origin),
                LayerMask = inputParameters.LayerMask.ValueOrDefault(LayerMask),
                QueryTriggerInteraction = inputParameters.QueryTriggerInteraction.ValueOrDefault(QueryTriggerInteraction)
            };
        }
    }
}
