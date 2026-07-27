using Cysharp.Threading.Tasks;
using SaintsField;
using SaintsField.Playa;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Paps.GameSetup
{
    [DefaultExecutionOrder(-10000)]
    public class GameSetupManager : MonoBehaviour
    {
        public static GameSetupManager Instance { get; private set; }

        [SerializeField] private GameSetupPipeline _pipeline;
        [SerializeField] private SaintsInterface<IPreGameSetupInitialization>[] _orderedPreSetupInitializations;

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            ExecutePreSetupInitializations();
        }

        public async UniTask Setup(CancellationToken cancellationToken)
        {
            await _pipeline.Execute(cancellationToken);
        }

        private void ExecutePreSetupInitializations()
        {
            for(int i = 0; i < _orderedPreSetupInitializations.Length; i++)
            {
                _orderedPreSetupInitializations[i].I.PreGameSetupInitialize();
            }
        }

        [Button(), HideIf(EMode.Play)]
        private void FindAllPreSetupInitializations()
        {
            var items = FindObjectsByType<MonoBehaviour>()
                .Where(m => m is IPreGameSetupInitialization)
                .Select(m => m as IPreGameSetupInitialization)
                .ToArray();

            _orderedPreSetupInitializations = new SaintsInterface<IPreGameSetupInitialization>[items.Length];

            for(int i = 0; i < items.Length; i++)
            {
                _orderedPreSetupInitializations[i] = new SaintsInterface<IPreGameSetupInitialization>(items[i] as MonoBehaviour);
            }
        }
    }
}
