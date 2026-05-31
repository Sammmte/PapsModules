using Cysharp.Threading.Tasks;
using Paps.GameSetup;
using Paps.Levels;
using Paps.SceneLoading;
using System.Threading;
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
            DoEntry(Application.exitCancellationToken).Forget();
        }

        private async UniTaskVoid DoEntry(CancellationToken cancellationToken)
        {
            await SceneLoader.LoadAsync("Setup", LoadSceneMode.Additive);
            cancellationToken.ThrowIfCancellationRequested();
            await GameSetupManager.Instance.Setup(cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            await LevelManager.Instance.LoadLevel(_startupLevel, loadLevelOptions: _loadLevelOptions);
        }
    }
}
