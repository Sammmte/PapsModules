using System;
using UnityEngine;
using UnityEngine.Localization;

namespace Paps.Localization
{
    [Serializable]
    public class LocalizedText
    {
        [SerializeField] private LocalizedString _localizedString;

        public string Text => _localizedString.GetLocalizedString();

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
            _localizedString.SetReference(tableId, localizationId);
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
