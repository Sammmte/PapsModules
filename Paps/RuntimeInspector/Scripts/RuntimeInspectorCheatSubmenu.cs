using Cysharp.Threading.Tasks;
using Paps.Cheats;
using UnityEngine;
using UnityEngine.UIElements;

namespace Paps.RuntimeInspector.Cheats
{
    public class RuntimeInspectorCheatSubmenu : ICheatSubmenu
    {
        public string DisplayName => "Runtime Inspector";

        private VisualElement _mainContainer;
        private Button _showHierarchyButton;

        public VisualElement GetVisualElement()
        {
            return _mainContainer;
        }

        public async UniTask Load()
        {
            var runtimeInspectorManagerPrefab = await this.LoadAssetAsync<GameObject>("RuntimeInspectorManager");

            GameObject.Instantiate(runtimeInspectorManagerPrefab);

            _mainContainer = new VisualElement();

            _showHierarchyButton = new Button();
            _showHierarchyButton.text = "Show or hide hierarchy";
            _showHierarchyButton.clicked += SwitchHierarchyVisibility;

            _mainContainer.Add(_showHierarchyButton);
        }

        private void SwitchHierarchyVisibility()
        {
            if(RuntimeInspectorManager.Instance.IsHierarchyEnabled)
            {
                RuntimeInspectorManager.Instance.HideHierarchy();
            }
            else
            {
                RuntimeInspectorManager.Instance.ShowHierarchy();
            }
        }
    }
}
