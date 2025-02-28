using System;
using UnityEngine.Localization;

namespace Paps.Localization
{
    public class UnityLocalizationLocalizedText : LocalizedText
    {
        private readonly LocalizedString _localizedString;

        public override string Text => _localizedString.GetLocalizedString();
        public override event Action<string> OnTextChanged;

        public UnityLocalizationLocalizedText(LocalizedString localizedString)
        {
            _localizedString = localizedString;
            _localizedString.StringChanged += OnStringChanged;
        }

        private void OnStringChanged(string value)
        {
            OnTextChanged?.Invoke(value);
        }
    }
}
