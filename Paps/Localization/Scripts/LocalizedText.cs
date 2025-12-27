using System;
using UnityEngine;
using UnityEngine.Localization;

namespace Paps.Localization
{
    [Serializable]
    public class LocalizedText
    {
        public static readonly LocalizedText NO_STRING = new LocalizedText();

        [SerializeField] private LocalizedString _localizedString;

        public string TableId => _localizedString.TableReference;

        public string Text
        {
            get
            {
                try
                {
                    return _localizedString.GetLocalizedString();
                }
                catch
                {
                    return "NO_LOCALIZED_STRING";
                }
            }
        }

        private bool _hookedToStringChangedEvent;
        
        private Action<string> _internalOnTextChanged;
        public event Action<string> OnTextChanged
        {
            add
            {
                _internalOnTextChanged += value;
                EnsureIsHookedToStringChangedEvent();
            }

            remove
            {
                _internalOnTextChanged -= value;
                EnsureIsHookedToStringChangedEvent();
            }
        }

        internal LocalizedText() { }

        internal LocalizedText(string tableId, string localizationId)
        {
            _localizedString = new LocalizedString(tableId, localizationId);
            _localizedString.StringChanged += OnStringChanged;
        }

        private void EnsureIsHookedToStringChangedEvent()
        {
            if (!_hookedToStringChangedEvent)
            {
                _localizedString.StringChanged += OnStringChanged;
                _hookedToStringChangedEvent = true;
            }
        }
        
        private void OnStringChanged(string value)
        {
            _internalOnTextChanged?.Invoke(value);
        }
        
        public static implicit operator string(LocalizedText text) => text.Text;
    }
}
