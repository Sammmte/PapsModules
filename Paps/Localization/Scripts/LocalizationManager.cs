using UnityEngine.Localization.Settings;
using Paps.UnityExtensions;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Paps.Localization
{
    public class LocalizationManager : MonoBehaviour
    {
        public static LocalizationManager Instance { get; private set; }

        private Language[] _availableLanguages;

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public async UniTask Initialize()
        {
            await LocalizationSettings.InitializationOperation.ToUniTask();
            await UniTask.NextFrame(); // this prevents errors from calling localized strings the same frame initialization finishes

            _availableLanguages = LocalizationSettings.AvailableLocales.Locales.ToArray(l => new Language(l.Identifier.Code, l.LocaleName));
        }

        public Language[] GetLanguages()
        {
            return _availableLanguages;
        }

        public LocalizedText GetLocalizedText(string tableId, string localizationId)
        {
            return new LocalizedText(tableId, localizationId);
        }

        public Language GetTextLanguage()
        {
            var id = LocalizationSettings.SelectedLocale.Identifier.Code;
            var name = LocalizationSettings.SelectedLocale.LocaleName;

            return new Language(id, name);
        }

        public void SetTextLanguage(string languageId)
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale(languageId);
        }
    }
}
