using Paps.Optionals;
using System;
using UnityEngine;

namespace Paps.ScreenTransitions
{
    [Serializable]
    public struct ScreenTransitionParameters
    {
        [SerializeField] public Optional<float> Duration;
        [SerializeField] public Optional<bool> UseUnscaledTime;
    }
}
