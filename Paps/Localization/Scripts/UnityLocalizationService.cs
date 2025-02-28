using System.Linq;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace Paps.Localization
{
    public class UnityLocalizationService : ILocalizationService
    {
        public Language[] GetLanguages()
        {
            return LocalizationSettings.AvailableLocales.Locales.Select(l => new Language(l.Identifier.Code, l.LocaleName)).ToArray();
        }

        public LocalizedText GetLocalizedText(string tableId, string localizationId)
        {
            return new UnityLocalizationLocalizedText(new LocalizedString(tableId, localizationId));
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
