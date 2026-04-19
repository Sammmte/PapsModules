using System;
using UnityEngine;

namespace Paps.Localization
{
    [Serializable]
    public struct LocalizedText
    {
        [field: SerializeField] public string TableId { get; set; }
        [field: SerializeField] public string LocalizationId { get; set;}

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

        public LocalizedText(string tableId, string localizationId)
        {
            TableId = tableId;
            LocalizationId = localizationId;
        }

        public static implicit operator string(LocalizedText text) => text.Text;
    }
}
