using System;
using UnityEngine;
using UnityEngine.Localization;

namespace Paps.Localization
{
    [Serializable]
    public struct SerializableLocalizedText
    {
        [SerializeField] private LocalizedString _localizedString;

        private LocalizedText _localizedText;

        public LocalizedText GetLocalizedText()
        {
            if(_localizedText == null) 
                _localizedText = new UnityLocalizationLocalizedText(_localizedString);

            return _localizedText;
        }
    }
}
