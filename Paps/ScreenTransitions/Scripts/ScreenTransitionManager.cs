using Cysharp.Threading.Tasks;
using System;

namespace Paps.ScreenTransitions
{
    public static class ScreenTransitionManager
    {
        public static async UniTask Play<T>(T transition, Func<UniTask> onPlayInFinished = null) where T : IScreenTransition
        {
            await transition.PlayIn();

            if(onPlayInFinished != null)
                await onPlayInFinished();

            await transition.PlayOut();
        }
    }
}
