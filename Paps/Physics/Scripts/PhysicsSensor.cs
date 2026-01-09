using Paps.Optionals;
using Paps.UnityExtensions;
using Paps.ValueReferences;
using SaintsField.Playa;
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
        [SerializeField] private ValueReference<LayerMask> _filterLayerMask;
        [field: SerializeField] public QueryTriggerInteraction QueryTriggerInteraction { get; private set; }
        [SerializeField] private Transform _origin;
        
        public Vector3 Origin => _origin.position;

        public LayerMask LayerMask => _filterLayerMask;
        
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

        public TempReadOnlyBufferSegment<Collider> Sense(OverrideParameters overrideParameters = default)
        {
            ClearBuffer();
            
            var count = Execute(CollidersBuffer, GetFinalParameters(overrideParameters));

            return new TempReadOnlyBufferSegment<Collider>(_collidersBuffer, count);
        }

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
