using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Paps.ScreenTransitions
{
    public abstract class ScreenTransition : MonoBehaviour
    {
        public abstract UniTask PlayIn(ScreenTransitionParameters parameters);
        public abstract UniTask PlayOut(ScreenTransitionParameters parameters);
    }
}
