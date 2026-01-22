using Paps.UnityPrefs;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Paps.Favorites
{
    public class FavoritesWindow : EditorWindow
    {
        private class ItemSelectManipulator : MouseManipulator
        {
            public ItemSelectManipulator()
            {
                activators.Add(new ManipulatorActivationFilter() { button = MouseButton.LeftMouse, clickCount = 2 });
            }

            protected override void RegisterCallbacksOnTarget()
            {
                target.RegisterCallback<MouseDownEvent>(OnMouseDown);
            }

            protected override void UnregisterCallbacksFromTarget()
            {
                target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
            }

            private void OnMouseDown(MouseDownEvent ev)
            {
                if(!CanStartManipulation(ev))
                    return;

                var favoriteProvider = target as IFavoriteProvider;

                if(favoriteProvider.Favorite.Object == null)
                    return;

                EditorGUIUtility.PingObject(favoriteProvider.Favorite.Object);
            }
        }

        private const string DRAG_GENERIC_DATA_KEY = "DRAG_FAVORITES";
        private static readonly Lazy<UnityPref> WINDOW_DATA_PREF = new Lazy<UnityPref>(() => UnityPrefs.UnityPrefs.GetPref(UnityPrefType.UserProjectPrefs, FavoritesManager.PREFS_SCOPE));

        [SerializeField, HideInInspector] private string _windowId;
        [SerializeField] private VisualTreeAsset _windowVTA;
        [SerializeField] private VisualTreeAsset _assetCellElementVTA;
        [SerializeField] private VisualTreeAsset _typeCellElementVTA;
        [SerializeField] private VisualTreeAsset _sceneCellElementVTA;

        private VisualElement _mainContainer;
        private DropdownField _groupsDropdown;
        private TextField _newGroupField;
        private Button _addGroupButton;
        private Button _deleteGroupButton;
        private MultiColumnListView _itemsContainer;
        
        private string _currentGroupId;

        [MenuItem("Paps/Favorites/Window")]
        public static void ShowWindow()
        {
            var window = ScriptableObject.CreateInstance<FavoritesWindow>();

            window._windowId = Guid.NewGuid().ToString();

            window.titleContent.text = "Favorites";
            window.titleContent.image = EditorGUIUtility.IconContent("d_Favorite").image;

            window.Show();
        }

        private void CreateGUI()
        {
            FavoritesManager.OnDataChanged -= OnDataChanged;
            FavoritesManager.OnDataChanged += OnDataChanged;

            _currentGroupId = LoadActiveGroup();

            var parent = _windowVTA.CloneTree();

            _mainContainer = parent.Children().First();

            parent.Remove(_mainContainer);

            AddStylesheets(_mainContainer);

            _groupsDropdown = _mainContainer.Q<DropdownField>("GroupsDropdown");
            _newGroupField = _mainContainer.Q<TextField>("NewGroupField");
            _addGroupButton = _mainContainer.Q<Button>("AddGroupButton");
            _deleteGroupButton = _mainContainer.Q<Button>("DeleteGroupButton");
            _itemsContainer = _mainContainer.Q<MultiColumnListView>();

            InitializeGroupsDropdown();
            InitializeNewGroupField();
            InitializeAddGroupButton();
            InitializeDeleteGroupButton();
            InitializeItemsContainer();

            UpdateCurrentGroup(_currentGroupId);

            rootVisualElement.Add(_mainContainer);
        }

        private void OnDataChanged()
        {
            var groups = FavoritesManager.GetGroups();
            _groupsDropdown.choices = groups;

            var carrySelected = true;
            
            if(!groups.Contains(_currentGroupId))
            {
                _currentGroupId = FavoritesManager.DEFAULT_GROUP_ID;
                carrySelected = false;
            }

            List<FavoriteObject> previouslySelected = null;

            if(carrySelected)
                previouslySelected = GetSelectedFavorites();

            UpdateCurrentGroup(_currentGroupId);

            if(previouslySelected != null && previouslySelected.Count > 0)
                _itemsContainer.SetSelectionWithoutNotify(GetIndicesOf(previouslySelected));
        }

        private void UpdateCurrentGroup(string newActiveGroup)
        {
            _currentGroupId = newActiveGroup;
            _groupsDropdown.SetValueWithoutNotify(newActiveGroup);
            _deleteGroupButton.enabledSelf = newActiveGroup != FavoritesManager.DEFAULT_GROUP_ID;

            WINDOW_DATA_PREF.Value.Set(GetSaveKey(), _currentGroupId);

            UpdateItemsContainer();
        }

        private void InitializeGroupsDropdown()
        {
            _groupsDropdown.choices = FavoritesManager.GetGroups();
            _groupsDropdown.SetValueWithoutNotify(_currentGroupId);
            
            _groupsDropdown.RegisterValueChangedCallback(ev => UpdateCurrentGroup(ev.newValue));
        }

        private void InitializeNewGroupField()
        {
            _newGroupField.RegisterValueChangedCallback(ev =>
            {
                OnNewGroupSubmitted(ev.newValue);
                HideNewGroupView();
            });
        }

        private void OnNewGroupSubmitted(string newGroup)
        {
            if(!FavoritesManager.TryAddGroup(newGroup))
            {
                Debug.LogWarning($"Group with id {newGroup} already exists!");
                return;
            }

            UpdateCurrentGroup(newGroup);
        }

        private void InitializeAddGroupButton()
        {
            _addGroupButton.clicked += SwitchNewGroupView;
        }

        private void ShowNewGroupView()
        {
            _newGroupField.style.display = DisplayStyle.Flex;
            _groupsDropdown.style.display = DisplayStyle.None;

            _newGroupField.SetValueWithoutNotify(string.Empty);
            _newGroupField.Focus();
        }

        private void HideNewGroupView()
        {
            _newGroupField.style.display = DisplayStyle.None;
            _groupsDropdown.style.display = DisplayStyle.Flex;
        }

        private void SwitchNewGroupView()
        {
            if(_newGroupField.style.display == DisplayStyle.Flex)
            {
                HideNewGroupView();
            }
            else
            {
                ShowNewGroupView();
            }
        }

        private void InitializeDeleteGroupButton()
        {
            _deleteGroupButton.clicked += ShowDeleteDialog;
        }

        private void ShowDeleteDialog()
        {
            if(EditorUtility.DisplayDialog(
                "Remove favorite group", $"Are you sure you want to delete {_currentGroupId} group?",
                "Accept", "Cancel"))
            {
                DeleteCurrentGroup();
            }
        }

        private void DeleteCurrentGroup()
        {
            FavoritesManager.DeleteGroup(_currentGroupId);
        }

        private void InitializeItemsContainer()
        {
            var objectColumn = _itemsContainer.columns["Object"];
            var typeColumn = _itemsContainer.columns["Type"];
            var sceneColumn = _itemsContainer.columns["Scene"];

            objectColumn.makeCell += CreateObjectCell;
            objectColumn.bindCell += BindObjectCell;
            objectColumn.unbindCell += UnbindObjectCell;

            typeColumn.makeCell += CreateTypeCell;
            typeColumn.bindCell += BindTypeCell;
            typeColumn.unbindCell += UnbindTypeCell;

            sceneColumn.makeCell += CreateSceneCell;
            sceneColumn.bindCell += BindSceneCell;
            sceneColumn.unbindCell += UnbindSceneCell;

            _itemsContainer.canStartDrag += ValidateDragStart;
            _itemsContainer.setupDragAndDrop += SetupDragAndDrop;
            _itemsContainer.dragAndDropUpdate += OnDragAndDropUpdate;
            _itemsContainer.handleDrop += OnHandleDrop;
        }

        private VisualElement CreateObjectCell()
        {
            var parent = _assetCellElementVTA.CloneTree();

            var cell = parent.Q<ObjectCellElement>();

            parent.Remove(cell);

            cell.Initialize();

            cell.AddManipulator(new ContextualMenuManipulator(OnItemOptions));
            cell.AddManipulator(new ItemSelectManipulator());

            return cell;
        }

        private void BindObjectCell(VisualElement element, int index)
        {
            var objectCell = element as ObjectCellElement;

            objectCell.SetData(FavoritesManager.GetFavorite(_currentGroupId, index));
        }

        private void UnbindObjectCell(VisualElement element, int index)
        {
            var objectCell = element as ObjectCellElement;

            objectCell.CleanUp();
        }

        private VisualElement CreateTypeCell()
        {
            var parent = _typeCellElementVTA.CloneTree();

            var cell = parent.Q<ObjectTypeCellElement>();

            parent.Remove(cell);

            cell.Initialize();

            cell.AddManipulator(new ContextualMenuManipulator(OnItemOptions));
            cell.AddManipulator(new ItemSelectManipulator());

            return cell;
        }

        private void BindTypeCell(VisualElement element, int index)
        {
            var cell = element as ObjectTypeCellElement;

            var favoriteObject = FavoritesManager.GetFavorite(_currentGroupId, index);

            cell.SetData(favoriteObject);
        }

        private void UnbindTypeCell(VisualElement element, int index)
        {
            var cell = element as ObjectTypeCellElement;

            cell.CleanUp();
        }

        private VisualElement CreateSceneCell()
        {
            var parent = _sceneCellElementVTA.CloneTree();

            var cell = parent.Q<ObjectSceneCellElement>();

            parent.Remove(cell);

            cell.Initialize();

            cell.AddManipulator(new ContextualMenuManipulator(OnItemOptions));
            cell.AddManipulator(new ItemSelectManipulator());

            return cell;
        }

        private void BindSceneCell(VisualElement element, int index)
        {
            var cell = element as ObjectSceneCellElement;

            var favoriteObject = FavoritesManager.GetFavorite(_currentGroupId, index);

            cell.SetData(favoriteObject);
        }

        private void UnbindSceneCell(VisualElement element, int index)
        {
            var cell = element as ObjectSceneCellElement;

            cell.CleanUp();
        }

        private void UpdateItemsContainer()
        {
            FavoritesManager.ResolveAllUnresolved();

            _itemsContainer.itemsSource = FavoritesManager.GetFavoritesOf(_currentGroupId);
            
            _itemsContainer.RefreshItems();
        }

        private string LoadActiveGroup()
        {
            return WINDOW_DATA_PREF.Value.Get(GetSaveKey(), FavoritesManager.DEFAULT_GROUP_ID);
        }

        private void AddStylesheets(VisualElement element)
        {
            foreach(var stylesheet in _windowVTA.stylesheets)
            {
                element.styleSheets.Add(stylesheet);
            }
        }

        private bool ValidateDragStart(CanStartDragArgs args)
        {
            return true;
        }

        private StartDragArgs SetupDragAndDrop(SetupDragAndDropArgs args)
        {
            var startDragArgs = new StartDragArgs("Drag Favorites", DragVisualMode.Move);

            var favorites = _itemsContainer.itemsSource as List<FavoriteObject>;

            var favoritesToMove = args.selectedIds.Select(i => favorites[i]).ToList();

            startDragArgs.SetGenericData(DRAG_GENERIC_DATA_KEY, favoritesToMove);

            return startDragArgs;
        }

        private DragVisualMode OnDragAndDropUpdate(HandleDragAndDropArgs dragArgs)
        {
            if(HasValidDragAndDropData(dragArgs))
            {
                return DragVisualMode.Move;
            }

            return DragVisualMode.Rejected;
        }

        private DragVisualMode OnHandleDrop(HandleDragAndDropArgs dragArgs)
        {
            var list = GetFavoriteObjectsFromDragAndDrop(dragArgs);

            if(list != null)
            {
                FavoritesManager.SetOrAddFavoritesAt(_currentGroupId, list, dragArgs.insertAtIndex);
                return DragVisualMode.Move;
            }

            return DragVisualMode.Rejected;
        }

        private bool HasValidDragAndDropData(HandleDragAndDropArgs dragArgs)
        {
            var genericData = dragArgs.dragAndDropData.GetGenericData(DRAG_GENERIC_DATA_KEY);

            var valid = (dragArgs.dragAndDropData.entityIds != null && 
                dragArgs.dragAndDropData.entityIds.Count > 0) ||
                (genericData != null && genericData is List<FavoriteObject>);

            return valid;
        }

        private List<FavoriteObject> GetFavoriteObjectsFromDragAndDrop(HandleDragAndDropArgs dragArgs)
        {
            var genericData = dragArgs.dragAndDropData.GetGenericData(DRAG_GENERIC_DATA_KEY);

            if(genericData != null)
            {
                return genericData as List<FavoriteObject>;
            }

            if(dragArgs.dragAndDropData.entityIds != null && 
                dragArgs.dragAndDropData.entityIds.Count > 0)
            {
                GlobalObjectId[] ids = new GlobalObjectId[dragArgs.dragAndDropData.entityIds.Count];
                GlobalObjectId.GetGlobalObjectIdsSlow(dragArgs.dragAndDropData.entityIds.ToArray(), ids);
                return ids.Select(i => new FavoriteObject(i)).ToList();
            }

            return null;
        }

        private List<FavoriteObject> GetSelectedFavorites() => _itemsContainer.selectedItems
            .Select(o => o as FavoriteObject).ToList();

        private List<int> GetIndicesOf(List<FavoriteObject> favorites) => favorites
            .Select(f => _itemsContainer.itemsSource.IndexOf(f)).ToList();

        private void OnItemOptions(ContextualMenuPopulateEvent ev)
        {
            ev.menu.AppendAction("Remove", action =>
            {
                ShowRemoveItemsDialog();
            });

            var groups = FavoritesManager.GetGroups();

            foreach(var group in groups)
            {
                if(group == _currentGroupId)
                    continue;

                ev.menu.AppendAction($"Move/{group}", action =>
                {
                    FavoritesManager.Move(_currentGroupId, group, GetSelectedFavorites());
                });
            }
        }

        private void ShowRemoveItemsDialog()
        {
            if(EditorUtility.DisplayDialog("Remove favorites", "Are you sure?", "Accept", "Cancel"))
            {
                FavoritesManager.RemoveFavorites(_currentGroupId, GetSelectedFavorites());
            }
        }

        private string GetSaveKey() => $"favorites-window-active-group-{_windowId}";
    }
}
