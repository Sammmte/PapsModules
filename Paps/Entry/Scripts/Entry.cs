using Cysharp.Threading.Tasks;
using Paps.GameSetup;
using Paps.Levels;
using Paps.SceneLoading;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Paps.Entry
{
    public class Entry : MonoBehaviour
    {
        [SerializeField] private Level _startupLevel;
        [SerializeField] private LevelManager.LoadLevelOptions _loadLevelOptions;

        private void Awake()
        {
            DoEntry().Forget();
        }

        private async UniTaskVoid DoEntry()
        {
            await SceneLoader.LoadAsync("Setup", LoadSceneMode.Additive);
            await WaitForSetupProcess();
            await LevelManager.Instance.LoadLevel(_startupLevel, loadLevelOptions: _loadLevelOptions);
        }

        private UniTask WaitForSetupProcess()
        {
            return GameSetupManager.Instance.Setup();
        }
    }
}
