using System;
using UnityEngine;

namespace Paps.Localization
{
    [Serializable]
    public struct LocalizedText
    {
        [SerializeField] private LocalizationIdReference _localizationIdReference;

        public string TableId
        {
            get => _localizationIdReference.TableId;
            set => _localizationIdReference.TableId = value;
        }
        public string LocalizationId
        {
            get => _localizationIdReference.LocalizationId;
            set => _localizationIdReference.LocalizationId = value;
        }

        public string Text
        {
            get
            {
                LocalizationProfiling.GET_LOCALIZED_STRING_MARKER.Begin();
                var text = LocalizationManager.Instance.GetLocalizedString(TableId, LocalizationId);
                LocalizationProfiling.GET_LOCALIZED_STRING_MARKER.End();

                return text;
            }
        }

        public static implicit operator string(LocalizedText text) => text.Text;
    }
}
