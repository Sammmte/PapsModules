using Cysharp.Threading.Tasks;
using Paps.GameSetup;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Paps.DevelopmentTools.Runtime
{
    public class DebugGameSetupProcess : GameSetupProcess
    {
        [SerializeField] private EventSystem _eventSystemPrefab;

        public override async UniTask Setup(CancellationToken cancellationToken)
        {
#if CHEATS || UNITY_EDITOR
            var eventSystemInstance = Instantiate(_eventSystemPrefab);
            DontDestroyOnLoad(eventSystemInstance);
#if CHEATS && UNITY_ANDROID && !UNITY_EDITOR
            UnityEngine.InputSystem.InputSystem.EnableDevice(UnityEngine.InputSystem.Accelerometer.current);
#endif
#endif
        }
    }
}
