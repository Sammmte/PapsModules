using System;
using UnityEngine;

namespace Paps.Localization
{
    [Serializable]
    public struct LocalizedText
    {
        public static readonly LocalizedText INVALID = new LocalizedText();

        [SerializeField] private LocalizationIdReference _localizationIdReference;

        [NonSerialized] private LocalizedTextParameter[] _parameters;

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

        public bool IsValid => !string.IsNullOrEmpty(TableId) && !string.IsNullOrEmpty(LocalizationId);

        public string Text
        {
            get
            {
                if(_parameters == null)
                    return LocalizationManager.Instance.GetLocalizedString(TableId, LocalizationId);

                return LocalizationManager.Instance.GetLocalizedStringWithParameters(TableId, LocalizationId, _parameters);
            }
        }

        public LocalizedText(string tableId, string localizationId)
        {
            _localizationIdReference = new LocalizationIdReference()
            {
                TableId = tableId,
                LocalizationId = localizationId
            };

            _parameters = null;
        }

        public LocalizedText(string tableId, string localizationId, params LocalizedTextParameter[] parameters) : this(tableId, localizationId)
        {
            _parameters = parameters;
        }

        public LocalizedText WithParameters(params LocalizedTextParameter[] parameters)
        {
            var newLocalizedText = new LocalizedText
            {
                _localizationIdReference = this._localizationIdReference,
                _parameters = parameters
            };

            return newLocalizedText;
        }

        public static implicit operator string(LocalizedText text) => text.Text;
    }

    public readonly struct LocalizedTextParameter
    {
        public static LocalizedTextParameter FromValue<T>(string name, T value) => new LocalizedTextParameter(name, value.ToString());
        public static LocalizedTextParameter FromValue(string name, string value) => new LocalizedTextParameter(name, value);

        public string Name { get; }
        public string Value { get; }

        private LocalizedTextParameter(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}
