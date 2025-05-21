using Paps.Optionals;
using System;
using UnityEngine;

namespace Paps.Physics
{
    public class RaycastSensor : CastPhysicsSensor
    {
        public struct OverrideParameters
        {
            public Optional<Vector3> Origin;
            public Optional<Vector3> Direction;
            public Optional<float> Distance;
            public Optional<LayerMask> LayerMask;
            public Optional<QueryTriggerInteraction> QueryTriggerInteraction;
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
            if (MaxResults == 1)
            {
                if (PhysicsHelper.Raycast(
                        _overrideParameters.Origin.ValueOrDefault(Origin),
                        _overrideParameters.Direction.ValueOrDefault(Direction),
                        _overrideParameters.Distance.ValueOrDefault(Distance),
                        _overrideParameters.LayerMask.ValueOrDefault(LayerMask),
                        out RaycastHit hitInfo,
                        _overrideParameters.QueryTriggerInteraction.ValueOrDefault(QueryTriggerInteraction)
                    ))
                {
                    rayHits[0] = hitInfo;
                    return 1;
                }

                return 0;
            }

            return PhysicsHelper.Raycast(
                _overrideParameters.Origin.ValueOrDefault(Origin),
                _overrideParameters.Direction.ValueOrDefault(Direction),
                _overrideParameters.Distance.ValueOrDefault(Distance),
                _overrideParameters.LayerMask.ValueOrDefault(LayerMask),
                rayHits,
                _overrideParameters.QueryTriggerInteraction.ValueOrDefault(QueryTriggerInteraction)
            );
        }
    }
}