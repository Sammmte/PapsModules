using Cysharp.Threading.Tasks;
using IngameDebugConsole;
using Paps.Cheats;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;

namespace Paps.InGameDebugConsole.Cheats
{
    public class InGameDebugConsoleCheatSubmenu : ICheatSubmenu
    {
        private DebugLogManager _inGameConsole;

        private VisualElement _container;
        private Button _toggleVisibilityButton;

        public string DisplayName => "In Game Console";

        public InGameDebugConsoleCheatSubmenu()
        {
            _container = new VisualElement();

            _toggleVisibilityButton = new Button();
            _toggleVisibilityButton.text = "Show Or Hide";
            _toggleVisibilityButton.style.fontSize = 40;
            _toggleVisibilityButton.clicked += ShowOrHide;

            _container.Add(_toggleVisibilityButton);
        }

        private void ShowOrHide()
        {
            if(_inGameConsole.gameObject.activeSelf)
                Hide();
            else
                Show();
        }

        private void Hide() => _inGameConsole.gameObject.SetActive(false);
        private void Show() => _inGameConsole.gameObject.SetActive(true);

        public async UniTask Load()
        {
            var debugConsolePrefab = await Addressables.LoadAssetAsync<GameObject>("InGameDebugConsolePrefab");
            _inGameConsole = Object.Instantiate(debugConsolePrefab).GetComponent<DebugLogManager>();
            Object.DontDestroyOnLoad(_inGameConsole);
            Hide();
        }

        public VisualElement GetVisualElement()
        {
            return _container;
        }
    }
}
