using Cysharp.Threading.Tasks;
using System;

namespace Paps.ScreenTransitions
{
    public static class ScreenTransitionExtensions
    {
        public static UniTask PlayIn(this ScreenTransitionConfiguration config)
        {
            return ScreenTransitionManager.Instance.PlayIn(config);
        }

        public static UniTask PlayOut(this ScreenTransitionConfiguration config)
        {
            return ScreenTransitionManager.Instance.PlayOut(config);
        }

        public static UniTask Play(this ScreenTransitionConfiguration config, Func<UniTask> onPlayInFinished = null)
        {
            return ScreenTransitionManager.Instance.Play(config, onPlayInFinished);
        }
    }
}
