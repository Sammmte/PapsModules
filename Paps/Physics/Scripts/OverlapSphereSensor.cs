using Paps.Optionals;
using Paps.UnityExtensions;
using System;
using UnityEngine;

namespace Paps.Physics
{
    public class OverlapSphereSensor : OverlapPhysicsSensor
    {
        public struct OverrideParameters
        {
            public Optional<Vector3> Origin;
            public Optional<float> Radius;
            public Optional<LayerMask> LayerMask;
            public Optional<QueryTriggerInteraction> QueryTriggerInteraction;
        }
        [field: SerializeField] public float Radius { get; private set; }
        
        private OverrideParameters _overrideParameters;
        
        public new TempReadOnlyBufferSegment<Collider> Sense(OverrideParameters overrideParameters = default)
        {
            _overrideParameters = overrideParameters;

            var result = base.Sense();

            _overrideParameters = default;

            return result;
        }
        
        protected override int Execute(Collider[] resultsBuffer, PhysicsSensor.OverrideParameters baseFinalParameters)
        {
            var finalParameters = GetFinalParameters(baseFinalParameters, _overrideParameters);
            
            return PhysicsHelper.OverlapSphere(
                finalParameters.Origin,
                finalParameters.Radius,
                resultsBuffer,
                finalParameters.LayerMask,
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
                Radius = overrideParameters.Radius.ValueOrDefault(Radius),
            };
        }
    }
}