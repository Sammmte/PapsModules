namespace Paps.Localization
{
    public interface ILocalizationService
    {
        public Language GetTextLanguage();
        public void SetTextLanguage(string languageId);
        public Language[] GetLanguages();
        public LocalizedText GetLocalizedText(string tableId, string localizationId);
    }
}
