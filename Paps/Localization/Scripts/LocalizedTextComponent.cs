using TMPro;
using UnityEngine;

namespace Paps.Localization
{
    public class LocalizedTextComponent : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _textUIComponent;
        [SerializeField] private LocalizedText _localizedText;
        
        private void Awake()
        {
            SetLocalizedText(_localizedText);
        }

        public void SetLocalizedText(LocalizedText localizedText)
        {
            if (_localizedText != null)
            {
                _localizedText.OnTextChanged -= UpdateText;
            }
            
            _localizedText = localizedText;
            _localizedText.OnTextChanged += UpdateText;
            
            UpdateText(_localizedText.Text);
        }

        private void UpdateText(string text)
        {
            _textUIComponent.text = text;
        }

        private void OnValidate()
        {
            var textMeshProUGUI = GetComponent<TextMeshProUGUI>();

            if (textMeshProUGUI)
                _textUIComponent = textMeshProUGUI;
            else
                _textUIComponent = null;
        }
    }
}