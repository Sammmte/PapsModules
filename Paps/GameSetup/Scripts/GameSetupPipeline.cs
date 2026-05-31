using Cysharp.Threading.Tasks;
using Paps.Logging;
using System;
using System.Linq;
using System.Threading;
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

        public async UniTask Execute(CancellationToken cancellationToken)
        {
            foreach (var step in _parallelSteps)
            {
                await UniTask.WhenAll(step.Processes.Select(async p =>
                {
                    if(p == null)
                    {
                        this.LogWarning($"Found GameSetupProcess being null on step: {step.Name}");
                        return;
                    }

                    await p.Setup(cancellationToken);

                    cancellationToken.ThrowIfCancellationRequested();
                }));
            }
        }

    }
}
