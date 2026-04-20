using Cysharp.Threading.Tasks;
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

        public async UniTask Setup()
        {
            await _pipeline.Execute();
        }
    }
}
