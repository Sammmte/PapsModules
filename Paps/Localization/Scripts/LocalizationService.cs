using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using Paps.UnityExtensions;

namespace Paps.Localization
{
    public class LocalizationService
    {
        public Language[] GetLanguages()
        {
            return LocalizationSettings.AvailableLocales.Locales.ToArray(l => new Language(l.Identifier.Code, l.LocaleName));
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
