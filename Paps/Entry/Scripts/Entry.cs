using Cysharp.Threading.Tasks;
using Paps.LevelSetup;
using Paps.SceneLoading;
using Paps.StartupSetup;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Paps.Entry
{
    public class Entry : MonoBehaviour
    {
        [SerializeField] private Level _startupLevel;
        [SerializeField] private LevelSetupper.LoadLevelOptions _loadLevelOptions;

        private void Awake()
        {
            DoEntry().Forget();
        }

        private async UniTaskVoid DoEntry()
        {
            await SceneLoader.LoadAsync("Setup", LoadSceneMode.Additive);
            await WaitForSetupProcess();
            await LevelSetupper.Instance.LoadAndSetupInitialLevel(_startupLevel, _loadLevelOptions);
        }

        private UniTask WaitForSetupProcess()
        {
            var setupper = FindFirstObjectByType<StartupSetupper>();

            return setupper.Setup();
        }
    }
}
