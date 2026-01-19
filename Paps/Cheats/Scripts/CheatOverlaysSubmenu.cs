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
        private Dictionary<string, CheatOverlayState> _overlaysState;
        private CheatOverlayStateRepository _stateRepository = new CheatOverlayStateRepository();

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

            _overlaysState = LoadOverlaysState();

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

            ApplyOverlaysState();

            _screenElement.OnOverlayPositionChanged += OnOverlayPositionChanged;
            Application.quitting += SaveOnQuit;
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
            SetVisibleState(overlay.Id, visible);

            ApplyVisibilityState(overlay);
        }

        private void OnOverlayPositionChanged(ICheatOverlay overlay, Vector2 position)
        {
            SetPositionState(overlay.Id, position);
        }

        private void ApplyVisibilityState(ICheatOverlay overlay)
        {
            var visible = _overlaysState[overlay.Id].Visible;

            if(visible)
            {
                _screenElement.AddOverlay(overlay);

                overlay.OnShow();
            }
            else
            {
                _screenElement.RemoveOverlay(overlay);

                overlay.OnHide();
            }
        }

        private void ApplyPositionState(ICheatOverlay overlay)
        {
            var position = _overlaysState[overlay.Id].Position;

            _screenElement.SetOverlayPosition(overlay, position);
        }

        private Dictionary<string, CheatOverlayState> LoadOverlaysState()
        {
            var dictionary = new Dictionary<string, CheatOverlayState>(_overlays.Count);

            var loadedData = _stateRepository.Get();

            foreach(var data in loadedData)
            {
                dictionary[data.Key] = data.Value;
            }

            return dictionary;
        }

        private void SetVisibleState(string overlayId, bool visibleState)
        {
            var state = GetStateOf(overlayId);

            state.Visible = visibleState;

            _overlaysState[overlayId] = state;
        }

        private void SetPositionState(string overlayId, Vector2 position)
        {
            var state = GetStateOf(overlayId);

            state.Position = position;

            _overlaysState[overlayId] = state;
        }

        private CheatOverlayState GetStateOf(string overlayId)
        {
            if(_overlaysState.TryGetValue(overlayId, out var state))
                return state;

            CheatOverlayState newState = default;

            _overlaysState[overlayId] = newState;

            return newState;
        }

        private void SaveOnQuit()
        {
            Debug.Log("SAVING ON QUIT!");
            _stateRepository.Save(_overlaysState);
        }

        private void ApplyOverlaysState()
        {
            foreach(var keyValue in _overlaysState)
            {
                var overlay = GetOverlayById(keyValue.Key);

                ApplyVisibilityState(overlay);
                ApplyPositionState(overlay);
            }
        }

        private ICheatOverlay GetOverlayById(string id)
        {
            for(int i = 0; i < _overlays.Count; i++)
            {
                if(_overlays[i].Id == id)
                    return _overlays[i];
            }

            return null;
        }
    }
}
