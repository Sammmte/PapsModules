using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace Paps.GameSetup
{
    public class GameSetupManager : MonoBehaviour
    {
        public static GameSetupManager Instance { get; private set; }

        [SerializeField] private GameSetupPipeline _pipeline;

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public async UniTask Setup(CancellationToken cancellationToken)
        {
            await _pipeline.Execute(cancellationToken);
        }
    }
}
