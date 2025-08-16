using Cysharp.Threading.Tasks;
using Paps.GameSetup;
using Paps.GlobalProvisioning;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace Paps.Localization
{
    [CreateAssetMenu(menuName = "Paps/Setup/Localization")]
    public class LocalizationGameSetupProcess : GameSetupProcess
    {
        public override async UniTask Setup()
        {
            await LocalizationSettings.InitializationOperation.ToUniTask();
            await UniTask.NextFrame(); // this prevents errors from calling localized strings the same frame initialization finishes

            var localizationService = new LocalizationService();
            Locator.Create(localizationService);
        }
    }
}
