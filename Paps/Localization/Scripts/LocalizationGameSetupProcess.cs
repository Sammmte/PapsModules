using Cysharp.Threading.Tasks;
using Paps.GameSetup;
using UnityEngine;

namespace Paps.Localization
{
    [CreateAssetMenu(menuName = "Paps/Setup/Localization")]
    public class LocalizationGameSetupProcess : GameSetupProcess
    {
        public override async UniTask Setup()
        {
            await LocalizationManager.Instance.Initialize();
        }
    }
}
