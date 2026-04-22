using Cysharp.Threading.Tasks;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

namespace Paps.ScreenTransitions
{
    public class AlphaScreenTransition : ScreenTransition
    {
        [SerializeField] private Image _image;

        private Tween _tween;

        public override async UniTask PlayIn(ScreenTransitionParameters parameters)
        {
            _tween.Stop();

            _tween = Tween.Alpha(_image, 0, 1, duration: parameters.Duration, useUnscaledTime: parameters.UseUnscaledTime);

            await _tween;
        }

        public override async UniTask PlayOut(ScreenTransitionParameters parameters)
        {
            _tween.Stop();

            _tween = Tween.Alpha(_image, 1, 0, duration: parameters.Duration, useUnscaledTime: parameters.UseUnscaledTime);

            await _tween;
        }
    }
}
