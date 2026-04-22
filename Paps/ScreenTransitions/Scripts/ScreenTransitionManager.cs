using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Paps.ScreenTransitions
{
    public class ScreenTransitionManager : MonoBehaviour
    {
        public static ScreenTransitionManager Instance { get; private set; }

        private Dictionary<ScreenTransition, ScreenTransition> _screenTransitionInstancesByPrefab;

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            _screenTransitionInstancesByPrefab = new Dictionary<ScreenTransition, ScreenTransition>();
        }

        public async UniTask Play(ScreenTransitionConfiguration configuration, Func<UniTask> onPlayInFinished = null)
        {
            var playInParameters = configuration.PlayInParameters;
            var playOutParamaters = configuration.PlayOutParameters;

            SetDefaultValueOnParameters(ref playInParameters);
            SetDefaultValueOnParameters(ref playOutParamaters);

            var transition = GetCachedInstanceOrInstantiate(configuration.TransitionPrefab);

            transition.gameObject.SetActive(true);

            await transition.PlayIn(playInParameters);

            if(onPlayInFinished != null)
                await onPlayInFinished();

            await transition.PlayOut(playOutParamaters);

            transition.gameObject.SetActive(false);
        }

        private ScreenTransition GetCachedInstanceOrInstantiate(ScreenTransition prefab)
        {
            if(_screenTransitionInstancesByPrefab.TryGetValue(prefab, out var instance))
            {
                return instance;
            }

            var newInstance = Instantiate(prefab, transform);

            _screenTransitionInstancesByPrefab[prefab] = newInstance;

            return newInstance;
        }

        private void SetDefaultValueOnParameters(ref ScreenTransitionParameters parameters)
        {
            if (!parameters.Duration.HasValue)
                parameters.Duration = 0;

            if (!parameters.UseUnscaledTime.HasValue)
                parameters.UseUnscaledTime = true;
        }
    }
}
