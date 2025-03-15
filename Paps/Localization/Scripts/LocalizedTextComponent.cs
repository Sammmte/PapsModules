using TMPro;
using UnityEngine;

namespace Paps.Localization
{
    public class LocalizedTextComponent : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _textUIComponent;
        [SerializeField] private SerializableLocalizedText _localizedText;

        private LocalizedText _localizedTextInstance;

        private void Awake()
        {
            _localizedTextInstance = _localizedText.GetLocalizedText();
            _localizedTextInstance.OnTextChanged += UpdateText;
            
            UpdateText(_localizedTextInstance.Text);
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