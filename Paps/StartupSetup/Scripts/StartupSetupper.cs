using Cysharp.Threading.Tasks;
using Paps.GameSetup;
using UnityEngine;

namespace Paps.StartupSetup
{
    public class StartupSetupper : MonoBehaviour
    {
        [SerializeField] private GameSetupPipeline _setupPipeline;

        public async UniTask Setup()
        {
            await _setupPipeline.Execute();
        }
    }
}
