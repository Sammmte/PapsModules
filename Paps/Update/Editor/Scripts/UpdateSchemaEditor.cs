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
        [SerializeField] private VisualTreeAsset _editorVTA;
        [SerializeField] private VisualTreeAsset _frameSequenceFrameCellVTA;
        [SerializeField] private VisualTreeAsset _frameGroupsSequenceCellVTA;
        [SerializeField] private VisualTreeAsset _frameGroupsSequenceGroupItemVTA;

        private VisualElement _mainContainer;
        private PropertyField _updatablesCapacityField;
        private PropertyField _defaultGroupField;
        private MultiColumnListView _updateGroupsListView;
        private MultiColumnListView _frameSequenceListView;

        private SerializedProperty _defaultGroupProperty;
        private SerializedProperty _groupsProperty;
        private SerializedProperty _frameSequenceProperty;

        public override VisualElement CreateInspectorGUI()
        {
            _mainContainer = _editorVTA.CloneTree();

            _updatablesCapacityField = _mainContainer.Q<PropertyField>("UpdatablesCapacityField");
            _defaultGroupField = new PropertyField();
            _defaultGroupField.label = "Default Group";
            
            // Insert default group field before the list
            var capacityFieldParent = _updatablesCapacityField.parent;
            capacityFieldParent.Insert(capacityFieldParent.IndexOf(_updatablesCapacityField) + 1, _defaultGroupField);

            _updateGroupsListView = _mainContainer.Q<MultiColumnListView>("UpdateGroupsList");
            _frameSequenceListView = _mainContainer.Q<MultiColumnListView>("FrameSequenceList");

            _updatablesCapacityField.BindProperty(serializedObject.FindProperty("_updatableGroupCapacity"));
            _defaultGroupProperty = serializedObject.FindProperty("_defaultGroup");
            _defaultGroupField.BindProperty(_defaultGroupProperty);

            _groupsProperty = serializedObject.FindProperty("_groups");
            _frameSequenceProperty = serializedObject.FindProperty("_frameSequence");

            InitializeUpdateGroupsList();
            InitializeFrameSequenceList();

            return _mainContainer;
        }

        private void InitializeUpdateGroupsList()
        {
            var groupColumn = _updateGroupsListView.columns[0];
            
            // We only need one column for the group object reference now
            groupColumn.title = "Group Asset";
            groupColumn.makeCell += () => new ObjectField() { objectType = typeof(UpdateSchemaGroup) };
            groupColumn.bindCell += (element, index) =>
            {
                var field = element as ObjectField;
                field.BindProperty(_groupsProperty.GetArrayElementAtIndex(index));
            };
            groupColumn.unbindCell += (element, index) =>
            {
                var field = element as ObjectField;
                field.Unbind();
            };

            // Remove ID column if it exists or hide it
            if(_updateGroupsListView.columns.Count > 1)
            {
                _updateGroupsListView.columns[1].width = 0;
                _updateGroupsListView.columns[1].visible = false;
            }

            _updateGroupsListView.BindProperty(_groupsProperty);
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

            cell.Initialize(_frameGroupsSequenceGroupItemVTA, GetAvailableGroupsFor, GetAvailableGroups);

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

        private UpdateSchemaGroup[] GetAvailableGroupsFor(int frameGroupSequenceIndex)
        {
            var frameGroupsSequenceProperty = _frameSequenceProperty.GetArrayElementAtIndex(frameGroupSequenceIndex);

            var groupsSequenceProperty = frameGroupsSequenceProperty.FindPropertyRelative(nameof(FrameGroupsSequence.GroupsSequence));

            var availableGroups = GetAvailableGroups();

            var list = new List<UpdateSchemaGroup>(availableGroups);

            for(int i = 0; i < groupsSequenceProperty.arraySize; i++)
            {
                var groupItemProperty = groupsSequenceProperty.GetArrayElementAtIndex(i);
                var groupAsset = groupItemProperty.objectReferenceValue as UpdateSchemaGroup;

                if(groupAsset != null)
                {
                    list.Remove(groupAsset);
                }
            }

            return list.ToArray();
        }

        private HashSet<UpdateSchemaGroup> GetAvailableGroups()
        {
            var hashset = new HashSet<UpdateSchemaGroup>(_groupsProperty.arraySize + 1);

            if (_defaultGroupProperty.objectReferenceValue != null)
                hashset.Add(_defaultGroupProperty.objectReferenceValue as UpdateSchemaGroup);

            for(int i = 0; i < _groupsProperty.arraySize; i++)
            {
                var groupProperty = _groupsProperty.GetArrayElementAtIndex(i);
                var groupAsset = groupProperty.objectReferenceValue as UpdateSchemaGroup;

                if (groupAsset != null)
                    hashset.Add(groupAsset);
            }

            return hashset;
        }
    }
}
