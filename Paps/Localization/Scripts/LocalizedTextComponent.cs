using SaintsField;
using System;
using TMPro;
using UnityEngine;

namespace Paps.Localization
{
    public class LocalizedTextComponent : MonoBehaviour
    {
        [SerializeField, GetComponent] private TextMeshProUGUI _textUIComponent;
        [SerializeField] private LocalizedText _localizedText;

        private Action<Language> _onLanguageChangedAction;
        
        private void Awake()
        {
            _onLanguageChangedAction = OnLanguageChanged;
        }

        private void OnEnable()
        {
            LocalizationManager.Instance.OnLanguageChanged += _onLanguageChangedAction;
            UpdateText();
        }

        private void OnDisable()
        {
            LocalizationManager.Instance.OnLanguageChanged -= _onLanguageChangedAction;
        }

        private void OnDestroy()
        {
            LocalizationManager.Instance.OnLanguageChanged -= _onLanguageChangedAction;
        }

        public void SetLocalizedText(LocalizedText localizedText)
        {
            _localizedText = localizedText;
            
            UpdateText();
        }

        private void UpdateText()
        {
            _textUIComponent.text = _localizedText.Text;
        }

        private void OnLanguageChanged(Language language)
        {
            UpdateText();
        }
    }
}