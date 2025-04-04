using Cysharp.Threading.Tasks;
using Paps.UnityExtensions;
using System;
using System.Linq;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace Paps.Cheats
{
    [UxmlElement]
    public partial class CheatsUI : VisualElement
    {
        private ICheatSubmenu[] _cheatSubmenus;
        private VisualElement[] _enterSubmenuButtonsParents;
        private ICheatSubmenu _openedSubmenu;

        private ScrollView _buttonsContainer;
        private VisualElement _submenuContainerParent;
        private ScrollView _submenuContainer;

        private Label _submenuDisplayNameLabel;

        private Button _backButton, _closeButton;

        private InputAction _toggleVisibilityAction;

        public static event Action OnCheatsDisplayed;
        public static event Action OnCheatsHidden;

        public CheatsUI() { }

        public async UniTask Initialize()
        {
            _buttonsContainer = this.Q<ScrollView>("ButtonsContainer");
            _submenuContainerParent = this.Q("SubmenuContainerParent");
            _submenuContainer = this.Q<ScrollView>("SubmenuContainer");
            _submenuDisplayNameLabel = this.Q<Label>("SubmenuDisplayNameLabel");

            _backButton = this.Q<Button>("BackButton");
            _closeButton = this.Q<Button>("CloseButton");

            var cheatActions = await CheatsHelper.LoadAssetAsync<InputActionAsset>("CheatActions");
            var cheatSubmenuButtonVTA = await CheatsHelper.LoadAssetAsync<VisualTreeAsset>("CheatSubmenuButton");

            _toggleVisibilityAction = cheatActions.FindAction("ToggleVisibility");

            _backButton.clicked += HideSubmenu;
            _closeButton.clicked += Hide;

            _cheatSubmenus = await LoadCheatSubmenus();
            _enterSubmenuButtonsParents = _cheatSubmenus
                .Select(c => CheatSubmenuToButton(c, cheatSubmenuButtonVTA)).ToArray();

            foreach (var button in _enterSubmenuButtonsParents)
            {
                _buttonsContainer.Add(button);
            }

            _toggleVisibilityAction.Enable();
            _toggleVisibilityAction.started += ShowOrHide;
        }

        private VisualElement CheatSubmenuToButton(ICheatSubmenu cheatSubmenu, VisualTreeAsset cheatSubmenuButtonVTA)
        {
            var cheatSubmenuButtonParent = cheatSubmenuButtonVTA.CloneTree();

            var button = cheatSubmenuButtonParent.Q<Button>();
            var label = cheatSubmenuButtonParent.Q<AutoSizeLabel>();

            label.text = cheatSubmenu.DisplayName;

            button.clicked += () => OpenSubmenu(cheatSubmenu);
            button.style.fontSize = 40;

            return cheatSubmenuButtonParent;
        }

        private void ShowOrHide(InputAction.CallbackContext context)
        {
            if(style.display == DisplayStyle.Flex)
                Hide();
            else
                Show();
        }

        public void Show()
        {
            style.display = DisplayStyle.Flex;
            
            OnCheatsDisplayed?.Invoke();
        }

        public void Hide()
        {
            style.display = DisplayStyle.None;
            
            OnCheatsHidden?.Invoke();
        }

        private void OpenSubmenu(ICheatSubmenu submenu)
        {
            _openedSubmenu?.OnHide();

            _buttonsContainer.style.display = DisplayStyle.None;
            _submenuContainer.Add(submenu.GetVisualElement());
            _submenuContainerParent.style.display = DisplayStyle.Flex;
            _submenuDisplayNameLabel.text = submenu.DisplayName;

            _openedSubmenu = submenu;
            _openedSubmenu.OnShow();
        }

        private void HideSubmenu()
        {
            _buttonsContainer.style.display = DisplayStyle.Flex;
            _submenuContainer.Clear();
            _submenuContainerParent.style.display = DisplayStyle.None;

            _openedSubmenu.OnHide();
            _openedSubmenu = null;
        }

        private async UniTask<ICheatSubmenu[]> LoadCheatSubmenus()
        {
            await UniTask.SwitchToThreadPool();

            var submenuTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(ICheatSubmenu).IsAssignableFrom(t))
                .Where(t => t != typeof(ICheatSubmenu));

            await UniTask.SwitchToMainThread();

            var submenus = submenuTypes
                .Select(t => (ICheatSubmenu)Activator.CreateInstance(t))
                .ToArray();
            
            await UniTask.WhenAll(submenus.Select(s => s.Load()));

            return submenus;
        }
    }
}
