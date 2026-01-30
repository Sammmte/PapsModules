using UnityEngine;
using UnityEditor;
using EditorObject = UnityEditor.Editor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Paps.Update.Editor
{
    [CustomEditor(typeof(UpdateSchema<>), editorForChildClasses: true)]
    public class UpdateSchemaEditor : EditorObject
    {
        private const string NEW_GROUP_NAME_BASE = "NewGroup";

        [SerializeField] private VisualTreeAsset _editorVTA;
        [SerializeField] private VisualTreeAsset _updateGroupNameCellVTA;
        [SerializeField] private VisualTreeAsset _updateGroupIdCellVTA;
        [SerializeField] private VisualTreeAsset _frameSequenceFrameCellVTA;
        [SerializeField] private VisualTreeAsset _frameGroupsSequenceCellVTA;
        [SerializeField] private VisualTreeAsset _frameGroupsSequenceGroupItemVTA;

        private VisualElement _mainContainer;
        private PropertyField _updatablesCapacityField;
        private MultiColumnListView _updateGroupsListView;
        private MultiColumnListView _frameSequenceListView;

        private SerializedProperty _groupsProperty;
        private SerializedProperty _frameSequenceProperty;

        public override VisualElement CreateInspectorGUI()
        {
            _mainContainer = _editorVTA.CloneTree();

            _updatablesCapacityField = _mainContainer.Q<PropertyField>("UpdatablesCapacityField");
            _updateGroupsListView = _mainContainer.Q<MultiColumnListView>("UpdateGroupsList");
            _frameSequenceListView = _mainContainer.Q<MultiColumnListView>("FrameSequenceList");

            _updatablesCapacityField.BindProperty(serializedObject.FindProperty("_updatableGroupCapacity"));

            _groupsProperty = serializedObject.FindProperty("_groups");
            _frameSequenceProperty = serializedObject.FindProperty("_frameSequence");

            InitializeUpdateGroupsList();
            InitializeFrameSequenceList();

            return _mainContainer;
        }

        private void InitializeUpdateGroupsList()
        {
            var nameColumn = _updateGroupsListView.columns[0];
            var idColumn = _updateGroupsListView.columns[1];

            nameColumn.makeCell += CreateGroupNameCell;
            nameColumn.bindCell += BindGroupNameCell;
            nameColumn.unbindCell += UnbindGroupNameCell;

            idColumn.makeCell += CreateGroupIdCell;
            idColumn.bindCell += BindGroupIdCell;
            idColumn.unbindCell += UnbindGroupIdCell;

            _updateGroupsListView.itemsAdded += OnNewGroupAdded;

            _updateGroupsListView.BindProperty(_groupsProperty);
        }

        private VisualElement CreateGroupNameCell()
        {
            var parent = _updateGroupNameCellVTA.CloneTree();

            var cell = parent.Q<UpdateGroupNameCellElement>();

            parent.Remove(cell);

            cell.Initialize(GetAvailableGroups);

            return cell;
        }

        private void BindGroupNameCell(VisualElement element, int index)
        {
            var cell = element as UpdateGroupNameCellElement;

            var property = _groupsProperty.GetArrayElementAtIndex(index).FindPropertyRelative(nameof(UpdatableGroup.Name));

            cell.SetData(property);
        }

        private void UnbindGroupNameCell(VisualElement element, int index)
        {
            var cell = element as UpdateGroupNameCellElement;

            cell.CleanUp();
        }

        private VisualElement CreateGroupIdCell()
        {
            var parent = _updateGroupIdCellVTA.CloneTree();

            var cell = parent.Q<Label>();

            return cell;
        }

        private void BindGroupIdCell(VisualElement element, int index)
        {
            var cell = element as Label;

            cell.text = (index + 1).ToString();
        }

        private void UnbindGroupIdCell(VisualElement element, int index)
        {
            var cell = element as Label;

            cell.text = string.Empty;
        }

        private void OnNewGroupAdded(IEnumerable<int> groupIndices)
        {
            var addedGroupIndex = groupIndices.First();

            EditorApplication.delayCall += () =>
            {
                var rootElement = _updateGroupsListView.GetRootElementForIndex(addedGroupIndex);

                var nameCell = rootElement.Q<UpdateGroupNameCellElement>();

                nameCell.SerializedProperty.stringValue = Guid.NewGuid().ToString();
                nameCell.SerializedProperty.serializedObject.ApplyModifiedProperties();

                nameCell.ShowRenameView(NEW_GROUP_NAME_BASE);
            };
        }

        private void InitializeFrameSequenceList()
        {
            var frameColumn = _frameSequenceListView.columns[0];
            var groupsSequenceCell = _frameSequenceListView.columns[1];

            frameColumn.makeCell += CreateFrameCell;
            frameColumn.bindCell += BindFrameCell;
            frameColumn.unbindCell += UnbindFrameCell;

            groupsSequenceCell.makeCell += CreateGroupsSequenceCell;
            groupsSequenceCell.bindCell += BindGroupsSequenceCell;
            groupsSequenceCell.unbindCell += UnbindGroupsSequenceCell;

            _frameSequenceListView.BindProperty(_frameSequenceProperty);
        }

        private VisualElement CreateFrameCell()
        {
            var parent = _frameSequenceFrameCellVTA.CloneTree();

            var cell = parent.Q<Label>("FrameLabel");

            parent.Remove(cell);

            return cell;
        }

        private void BindFrameCell(VisualElement element, int index)
        {
            var cell = element as Label;

            cell.text = $"Frame {(index + 1).ToString()}";
        }

        private void UnbindFrameCell(VisualElement element, int index)
        {
            var cell = element as Label;

            cell.text = string.Empty;
        }

        private VisualElement CreateGroupsSequenceCell()
        {
            var parent = _frameGroupsSequenceCellVTA.CloneTree();

            var cell = parent.Q<FrameGroupsSequenceElement>();

            parent.Remove(cell);

            cell.Initialize(_frameGroupsSequenceGroupItemVTA, GetAvailableGroupsFor);

            return cell;
        }

        private void BindGroupsSequenceCell(VisualElement element, int index)
        {
            var cell = element as FrameGroupsSequenceElement;

            cell.SetData(_frameSequenceProperty.GetArrayElementAtIndex(index), index);
        }

        private void UnbindGroupsSequenceCell(VisualElement element, int index)
        {
            var cell = element as FrameGroupsSequenceElement;

            cell.CleanUp();
        }

        private string[] GetAvailableGroupsFor(int frameGroupSequenceIndex)
        {
            var frameGroupsSequenceProperty = _frameSequenceProperty.GetArrayElementAtIndex(frameGroupSequenceIndex);

            var groupsSequenceProperty = frameGroupsSequenceProperty.FindPropertyRelative(nameof(FrameGroupsSequence.GroupsSequence));

            var availableGroups = GetAvailableGroups();

            var list = new List<string>(availableGroups);

            for(int i = 0; i < groupsSequenceProperty.arraySize; i++)
            {
                var groupItemProperty = groupsSequenceProperty.GetArrayElementAtIndex(i);

                list.Remove(groupItemProperty.stringValue);
            }

            return list.ToArray();
        }

        private HashSet<string> GetAvailableGroups()
        {
            var hashset = new HashSet<string>(_groupsProperty.arraySize + 1);

            hashset.Add(UpdatableGroup.DEFAULT_GROUP_NAME);

            for(int i = 0; i < _groupsProperty.arraySize; i++)
            {
                var groupProperty = _groupsProperty.GetArrayElementAtIndex(i);

                var nameProperty = groupProperty.FindPropertyRelative(nameof(UpdatableGroup.Name));

                hashset.Add(nameProperty.stringValue);
            }

            return hashset;
        }
    }
}
