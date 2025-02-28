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
        [SerializeField] private ScriptableLevel _startupLevel;

        private void Awake()
        {
            EntryTask().Forget();
        }

        private async UniTask EntryTask()
        {
            await SceneLoader.LoadAsync("Setup", LoadSceneMode.Additive);
            await WaitForSetupProcess();
            await LevelSetupper.LoadAndSetupInitialLevel(_startupLevel);
        }

        private UniTask WaitForSetupProcess()
        {
            var setupper = FindFirstObjectByType<StartupSetupper>();

            return setupper.Setup();
        }
    }
}
