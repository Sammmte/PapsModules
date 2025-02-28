using Cysharp.Threading.Tasks;
using Paps.Cheats;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;

namespace Paps.FPSMonitor.Cheats
{
    public class FPSMonitorCheatSubmenu : ICheatSubmenu
    {
        private VisualElement _container;
        private Button _toggleVisibilityButton;
        private GameObject _fpsMonitorInstance;

        public string DisplayName => "FPS Monitor";

        public FPSMonitorCheatSubmenu()
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
            if(_fpsMonitorInstance.activeSelf)
                Hide();
            else
                Show();
        }

        private void Hide() => _fpsMonitorInstance.gameObject.SetActive(false);
        private void Show() => _fpsMonitorInstance.gameObject.SetActive(true);

        public async UniTask Load()
        {
            var fpsMonitorPrefab = await Addressables.LoadAssetAsync<GameObject>("FPSMonitorPrefab");
            _fpsMonitorInstance = Object.Instantiate(fpsMonitorPrefab);
            Object.DontDestroyOnLoad(_fpsMonitorInstance);
            Hide();
        }

        public VisualElement GetVisualElement()
        {
            return _container;
        }
    }
}
