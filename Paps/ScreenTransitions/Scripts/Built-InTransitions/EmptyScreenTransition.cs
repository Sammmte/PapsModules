using Cysharp.Threading.Tasks;

namespace Paps.ScreenTransitions
{
    public class EmptyScreenTransition : ScreenTransition
    {
        public override UniTask PlayIn(ScreenTransitionParameters parameters) => UniTask.CompletedTask;

        public override UniTask PlayOut(ScreenTransitionParameters parameters) => UniTask.CompletedTask;
    }
}
