using Cysharp.Threading.Tasks;
using Paps.Logging;
using System;
using System.Linq;
using UnityEngine;

namespace Paps.GameSetup
{
    public class GameSetupPipeline : ScriptableObject
    {
        [Serializable]
        private struct ParallelStep
        {
            [SerializeField] public string Name;
            [SerializeField] public GameSetupProcess[] Processes;
        }

        [SerializeField] private ParallelStep[] _parallelSteps;

        public async UniTask Execute()
        {
            foreach (var step in _parallelSteps)
            {
                await UniTask.WhenAll(step.Processes.Select(p =>
                {
                    if(p == null)
                    {
                        this.LogWarning($"Found GameSetupProcess being null on step: {step.Name}");
                        return UniTask.CompletedTask;
                    }

                    return p.Setup();
                }));
            }
        }

    }
}
