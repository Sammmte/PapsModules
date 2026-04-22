using UnityEngine;

namespace Paps.ScreenTransitions
{
    [CreateAssetMenu(menuName = "Paps/Screen Transitions/Configuration Asset")]
    public class ScreenTransitionConfiguration : ScriptableObject
    {
        [field: SerializeField] public ScreenTransition TransitionPrefab { get; private set; }
        [field: SerializeField] public ScreenTransitionParameters PlayInParameters { get; private set; }
        [field: SerializeField] public ScreenTransitionParameters PlayOutParameters { get; private set; }
    }
}
