using Cysharp.Threading.Tasks;
using Paps.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        [SerializeField] private int _acceptableMillisecondsProcess, _notSoAcceptableMillisecondsProcess;
        [SerializeField] private int _acceptableMillisecondsStep, _notSoAcceptableMillisecondsStep;
        [SerializeField] private int _acceptableMillisecondsPipeline, _notSoAcceptableMillisecondsPipeline;
        [SerializeField] private ParallelStep[] _parallelSteps;

#if UNITY_EDITOR || PAPS_LOG
        public async UniTask Execute()
        {
            var timeDictionary = CreateTimeDictionary();

            foreach (var step in _parallelSteps)
            {
                await UniTask.WhenAll(step.Processes.Select(p =>
                {
                    return UniTask.Create(async () =>
                    {
                        var stopWatch = Stopwatch.StartNew();
                        await p.Setup();
                        var elapsedMilliseconds = stopWatch.ElapsedMilliseconds;
                        stopWatch.Stop();
                        timeDictionary[step.Name][p] = elapsedMilliseconds;
                    });
                }));
            }

            LogProcessesTime(timeDictionary);
        }

        private Dictionary<string, Dictionary<GameSetupProcess, long>> CreateTimeDictionary()
        {
            var timeDictionary = new Dictionary<string, Dictionary<GameSetupProcess, long>>();

            foreach (var step in _parallelSteps)
            {
                var processTimeDictionary = new Dictionary<GameSetupProcess, long>();
                timeDictionary.Add(step.Name, processTimeDictionary);

                foreach (var process in step.Processes)
                    processTimeDictionary.Add(process, 0);
            }

            return timeDictionary;
        }

        private void LogProcessesTime(Dictionary<string, Dictionary<GameSetupProcess, long>> timeDictionary)
        {
            long fullTimeMilliseconds = 0;

            foreach (var stepWithProcessesTime in timeDictionary)
            {
                var processesWithTime = stepWithProcessesTime.Value;
                long stepTime = 0;

                foreach (var processWithTime in processesWithTime)
                {
                    var processName = processWithTime.Key.GetType().Name;
                    var processTime = processWithTime.Value;

                    fullTimeMilliseconds += processTime;
                    stepTime += processTime;

                    this.Log($"Process {processName} took <color={GetStringColorForTime(processTime, _acceptableMillisecondsProcess, _notSoAcceptableMillisecondsProcess)}>{processTime} milliseconds</color>");
                }

                this.Log($"Step {stepWithProcessesTime.Key} took <color={GetStringColorForTime(stepTime, _acceptableMillisecondsStep, _notSoAcceptableMillisecondsStep)}>{stepTime} milliseconds</color>");
            }

            this.Log($"Setup process took <color={GetStringColorForTime(fullTimeMilliseconds, _acceptableMillisecondsPipeline, _notSoAcceptableMillisecondsPipeline)}>{fullTimeMilliseconds} milliseconds</color>");
        }

        private string GetStringColorForTime(long time, int acceptable, int notSoAcceptable)
        {
            if (time <= acceptable)
                return "green";
            else if (time <= notSoAcceptable)
                return "yellow";
            else
                return "red";
        }
#else
        public async UniTask Execute()
        {
            foreach (var step in _parallelSteps)
            {
                await UniTask.WhenAll(step.Processes.Select(p => p.Setup()));
            }
        }
#endif

    }
}
