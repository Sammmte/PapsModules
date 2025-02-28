using Cysharp.Threading.Tasks;
using Paps.Cheats;
using UnityEngine;
using UnityEngine.UIElements;

namespace Paps.Logging.Cheats
{
    public class LoggingCheatSubmenu : ICheatSubmenu
    {
        public string DisplayName => "Logging";

        private VisualElement _container;
        private Toggle _logEnabledToggle;

        public LoggingCheatSubmenu()
        {
            _container = new VisualElement();

            _logEnabledToggle = new Toggle("Log Enabled");
            _logEnabledToggle.value = LogManager.LogEnabled;
            _logEnabledToggle.style.fontSize = 40;
            _logEnabledToggle.style.color = new StyleColor(Color.white);

            _logEnabledToggle.RegisterValueChangedCallback(ev => LogManager.LogEnabled = ev.newValue);

            _container.Add(_logEnabledToggle);
        }

        public VisualElement GetVisualElement()
        {
            return _container;
        }

        public async UniTask Load()
        {
            
        }
    }
}
