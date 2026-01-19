using Cysharp.Threading.Tasks;
using Paps.UnityExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Paps.Cheats
{
    public class CheatOverlaysSubmenu : ICheatSubmenu
    {
        private const string OVERLAY_NAME_ELEMENT_CLASS = "cheat-overlays-submenu__item__name-label";

        private List<ICheatOverlay> _overlays;

        private List<ICheatOverlay> _visibleOverlays = new List<ICheatOverlay>();

        private VisualTreeAsset _submenuVTA;
        private VisualElement _mainElement;
        private VisualElement _noElementsContainer;
        private MultiColumnListView _itemsContainer;
        private CheatOverlaysScreenElement _screenElement;

        public string DisplayName => "Overlays";

        public VisualElement GetVisualElement()
        {
            return _mainElement;
        }

        public async UniTask Load()
        {
            _overlays = await LoadOverlays();
            _submenuVTA = await this.LoadAssetAsync<VisualTreeAsset>("CheatOverlaysSubmenuUI");
            var screenUIDocumentPrefab = await this.LoadAssetAsync<GameObject>("CheatOverlaysUIDocument");

            _mainElement = _submenuVTA.CloneTree();

            _noElementsContainer = _mainElement.Q("NoElementsContainer");
            _itemsContainer = _mainElement.Q<MultiColumnListView>();

            if(_overlays.Count == 0)
            {
                _noElementsContainer.style.display = DisplayStyle.Flex;
                _itemsContainer.style.display = DisplayStyle.None;
                return;
            }
            else
            {
                _noElementsContainer.style.display = DisplayStyle.None;
            }

            InitializeColumns();

            _itemsContainer.itemsSource = _overlays;

            _itemsContainer.Rebuild();

            var uiDocumentGameObject = GameObject.Instantiate(screenUIDocumentPrefab);
            var uiDocument = uiDocumentGameObject.GetComponent<CheatOverlayUIDocument>();

            _screenElement = uiDocument.Initialize();
        }

        private void InitializeColumns()
        {
            var nameColumn = _itemsContainer.columns["Name"];
            var visibilityColumn = _itemsContainer.columns["Visible"];

            nameColumn.makeCell += CreateNameCell;
            nameColumn.bindCell += BindNameCell;
            nameColumn.unbindCell += UnbindNameCell;

            visibilityColumn.makeCell += CreateVisibilityCell;
            visibilityColumn.bindCell += BindVisibilityCell;
            visibilityColumn.unbindCell += UnbindVisibilityCell;
        }

        private async UniTask<List<ICheatOverlay>> LoadOverlays()
        {
            await UniTask.SwitchToThreadPool();

            var overlayTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(ICheatOverlay).IsAssignableFrom(t))
                .Where(t => t != typeof(ICheatOverlay));

            await UniTask.SwitchToMainThread();

            var overlays = overlayTypes
                .Select(t => (ICheatOverlay)Activator.CreateInstance(t))
                .ToList();
            
            await UniTask.WhenAll(overlays.Select(s => s.Load()));

            return overlays;
        }

        private VisualElement CreateNameCell()
        {
            var label = new Label();

            AddStylesheets(label);

            label.AddToClassList(OVERLAY_NAME_ELEMENT_CLASS);

            return label;
        }

        private void BindNameCell(VisualElement element, int index)
        {
            var label = element as Label;

            label.text = _overlays[index].DisplayName;
        }

        private void UnbindNameCell(VisualElement element, int index)
        {
            var label = element as Label;

            label.text = string.Empty;
        }

        private VisualElement CreateVisibilityCell()
        {
            var visibilityElement = new CheatOverlayVisibilityElement();

            AddStylesheets(visibilityElement);

            visibilityElement.Initialize();

            return visibilityElement;
        }

        private void BindVisibilityCell(VisualElement element, int index)
        {
            var visibilityElement = element as CheatOverlayVisibilityElement;

            visibilityElement.SetData(_overlays[index]);

            visibilityElement.OnValueChanged += OnOverlayVisibilityChanged;
        }

        private void UnbindVisibilityCell(VisualElement element, int index)
        {
            var visibilityElement = element as CheatOverlayVisibilityElement;

            visibilityElement.CleanUp();

            visibilityElement.OnValueChanged -= OnOverlayVisibilityChanged;
        }

        private void AddStylesheets(VisualElement element)
        {
            foreach(var styleSheet in _submenuVTA.stylesheets)
            {
                element.styleSheets.Add(styleSheet);
            }
        }

        private void OnOverlayVisibilityChanged(ICheatOverlay overlay, bool visible)
        {
            if(visible)
            {
                _visibleOverlays.Add(overlay);

                _screenElement.AddOverlay(overlay);

                overlay.OnShow();
            }
            else
            {
                _visibleOverlays.Remove(overlay);

                _screenElement.RemoveOverlay(overlay);

                overlay.OnHide();
            }
        }
    }
}
