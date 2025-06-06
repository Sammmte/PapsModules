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
        public override UniTask Setup()
        {
            return UniTask.Create(async () =>
            {
                await LocalizationSettings.InitializationOperation.ToUniTask();
                await UniTask.NextFrame(); // this prevents errors from calling localized strings the same frame initialization finishes

                var localizationService = new UnityLocalizationService();
                Locator.Create<ILocalizationService>(localizationService);
            });
        }
    }
}
