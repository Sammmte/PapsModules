using Paps.Optionals;
using System;
using UnityEngine;

namespace Paps.Physics
{
    public class RaycastSensor : CastPhysicsSensor
    {
        protected override int Execute(RaycastHit[] rayHits, CastPhysicsSensor.OverrideParameters finalParameters)
        {
            if (MaxResults == 1)
            {
                if (PhysicsHelper.Raycast(
                        finalParameters.Origin,
                        finalParameters.Direction,
                        finalParameters.Distance,
                        finalParameters.LayerMask,
                        out RaycastHit hitInfo,
                        finalParameters.QueryTriggerInteraction
                    ))
                {
                    rayHits[0] = hitInfo;
                    return 1;
                }

                return 0;
            }

            return PhysicsHelper.Raycast(
                finalParameters.Origin,
                finalParameters.Direction,
                finalParameters.Distance,
                finalParameters.LayerMask,
                rayHits,
                finalParameters.QueryTriggerInteraction,
                finalParameters.OrderByDistance
            );
        }
    }
}