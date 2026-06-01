using Cysharp.Threading.Tasks;
using Paps.GameSetup;
using System.Threading;
using UnityEngine;

namespace Paps.Localization
{
    [CreateAssetMenu(menuName = "Paps/Setup/Localization")]
    public class LocalizationGameSetupProcess : GameSetupProcess
    {
        [SerializeField] private string[] _preloadTableIds;

        public override async UniTask Setup(CancellationToken cancellationToken)
        {
            await LocalizationManager.Instance.Initialize(cancellationToken);

            await LocalizationManager.Instance.LoadTablesAsync(cancellationToken, _preloadTableIds);
        }
    }
}
