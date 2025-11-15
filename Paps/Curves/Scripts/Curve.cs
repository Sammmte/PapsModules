using SaintsField.Playa;
using System;
using UnityEngine;

namespace Paps.Curves
{
    [Serializable]
    public class Curve
    {
        [field: SerializeField] public CurveType Type { get; set; }
        [field: SerializeField, ShowIf(nameof(Type), CurveType.Custom)] 
        public AnimationCurve CustomCurve { get; set; }
        [field: SerializeField, ShowIf(nameof(Type), CurveType.Predefined)] 
        public PredefinedCurve PredefinedCurve { get; set; }
        
        public float Evaluate(float position)
        {
            switch (Type)
            {
                case CurveType.Custom:
                    return CustomCurve.Evaluate(position);
                case CurveType.Predefined:
                    return EvaluatePredefined(PredefinedCurve, position);
            }

            throw new InvalidOperationException($"Curve type {Type} invalid");
        }

        private static float EvaluatePredefined(PredefinedCurve predefinedCurve, float position)
        {
            switch (predefinedCurve)
            {
                case PredefinedCurve.Linear: return PredefinedCurveDefinitions.Linear_Tween(position);
				case PredefinedCurve.AntiLinear: return PredefinedCurveDefinitions.LinearAnti_Tween(position);
				case PredefinedCurve.EaseInQuadratic: return PredefinedCurveDefinitions.EaseIn_Quadratic(position);
				case PredefinedCurve.EaseOutQuadratic: return PredefinedCurveDefinitions.EaseOut_Quadratic(position);
				case PredefinedCurve.EaseInOutQuadratic: return PredefinedCurveDefinitions.EaseInOut_Quadratic(position);
				case PredefinedCurve.EaseInCubic: return PredefinedCurveDefinitions.EaseIn_Cubic(position);
				case PredefinedCurve.EaseOutCubic: return PredefinedCurveDefinitions.EaseOut_Cubic(position);
				case PredefinedCurve.EaseInOutCubic: return PredefinedCurveDefinitions.EaseInOut_Cubic(position);
				case PredefinedCurve.EaseInQuartic: return PredefinedCurveDefinitions.EaseIn_Quartic(position);
				case PredefinedCurve.EaseOutQuartic: return PredefinedCurveDefinitions.EaseOut_Quartic(position);
				case PredefinedCurve.EaseInOutQuartic: return PredefinedCurveDefinitions.EaseInOut_Quartic(position);
				case PredefinedCurve.EaseInQuintic: return PredefinedCurveDefinitions.EaseIn_Quintic(position);
				case PredefinedCurve.EaseOutQuintic: return PredefinedCurveDefinitions.EaseOut_Quintic(position);
				case PredefinedCurve.EaseInOutQuintic: return PredefinedCurveDefinitions.EaseInOut_Quintic(position);
				case PredefinedCurve.EaseInSinusoidal: return PredefinedCurveDefinitions.EaseIn_Sinusoidal(position);
				case PredefinedCurve.EaseOutSinusoidal: return PredefinedCurveDefinitions.EaseOut_Sinusoidal(position);
				case PredefinedCurve.EaseInOutSinusoidal: return PredefinedCurveDefinitions.EaseInOut_Sinusoidal(position);
				case PredefinedCurve.EaseInBounce: return PredefinedCurveDefinitions.EaseIn_Bounce(position);
				case PredefinedCurve.EaseOutBounce: return PredefinedCurveDefinitions.EaseOut_Bounce(position);
				case PredefinedCurve.EaseInOutBounce: return PredefinedCurveDefinitions.EaseInOut_Bounce(position);
				case PredefinedCurve.EaseInOverhead: return PredefinedCurveDefinitions.EaseIn_Overhead(position);
				case PredefinedCurve.EaseOutOverhead: return PredefinedCurveDefinitions.EaseOut_Overhead(position);
				case PredefinedCurve.EaseInOutOverhead: return PredefinedCurveDefinitions.EaseInOut_Overhead(position);
				case PredefinedCurve.EaseInExponential: return PredefinedCurveDefinitions.EaseIn_Exponential(position);
				case PredefinedCurve.EaseOutExponential: return PredefinedCurveDefinitions.EaseOut_Exponential(position);
				case PredefinedCurve.EaseInOutExponential: return PredefinedCurveDefinitions.EaseInOut_Exponential(position);
				case PredefinedCurve.EaseInElastic: return PredefinedCurveDefinitions.EaseIn_Elastic(position);
				case PredefinedCurve.EaseOutElastic: return PredefinedCurveDefinitions.EaseOut_Elastic(position);
				case PredefinedCurve.EaseInOutElastic: return PredefinedCurveDefinitions.EaseInOut_Elastic(position);
				case PredefinedCurve.EaseInCircular: return PredefinedCurveDefinitions.EaseIn_Circular(position);
				case PredefinedCurve.EaseOutCircular: return PredefinedCurveDefinitions.EaseOut_Circular(position);
				case PredefinedCurve.EaseInOutCircular: return PredefinedCurveDefinitions.EaseInOut_Circular(position);
				case PredefinedCurve.AlmostIdentity: return PredefinedCurveDefinitions.AlmostIdentity(position);
            }
            
            throw new InvalidOperationException($"Predefined curve {predefinedCurve} invalid");
        }
    }
}
