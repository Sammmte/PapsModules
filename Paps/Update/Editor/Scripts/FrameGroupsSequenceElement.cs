using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Paps.Update.Editor
{
    [UxmlElement]
    public partial class FrameGroupsSequenceElement : VisualElement
    {
        private ListView _listView;
        private VisualTreeAsset _groupItemVTA;
        private Func<int, string[]> _getAvailableGroups;

        private SerializedProperty _frameGroupSequenceProperty;
        private SerializedProperty _groupsSequenceProperty;

        private int _frameGroupsSequenceIndex;

        public void Initialize(VisualTreeAsset groupItemVTA, Func<int, string[]> getAvailableGroups)
        {
            _groupItemVTA = groupItemVTA;
            _getAvailableGroups = getAvailableGroups;

            _listView = this.Q<ListView>();

            InitializeListView();
        }

        private void InitializeListView()
        {
            _listView.makeItem += CreateItem;
            _listView.bindItem += BindItem;
            _listView.unbindItem += UnbindItem;

            _listView.onAdd += OnItemAdded;
        }

        private VisualElement CreateItem()
        {
            var parent = _groupItemVTA.CloneTree();

            var item = parent.Q<FrameGroupsSequenceItemElement>();

            parent.Remove(item);

            item.Initialize(_getAvailableGroups);

            return item;
        }

        private void BindItem(VisualElement element, int index)
        {
            var item = element as FrameGroupsSequenceItemElement;

            item.SetData(_groupsSequenceProperty.GetArrayElementAtIndex(index), _frameGroupsSequenceIndex);
        }

        private void UnbindItem(VisualElement element, int index)
        {
            var item = element as FrameGroupsSequenceItemElement;

            item.CleanUp();
        }

        public void SetData(SerializedProperty property, int frameGroupSequenceIndex)
        {
            CleanUp();

            _frameGroupsSequenceIndex = frameGroupSequenceIndex;
            _frameGroupSequenceProperty = property;
            _groupsSequenceProperty = _frameGroupSequenceProperty.FindPropertyRelative(nameof(FrameGroupsSequence.GroupsSequence));

            _listView.BindProperty(_groupsSequenceProperty);
        }

        public void CleanUp()
        {
            _listView.Unbind();

            _frameGroupsSequenceIndex = -1;
            _frameGroupSequenceProperty = null;
            _groupsSequenceProperty = null;
        }

        private void OnItemAdded(BaseListView listView)
        {
            var availableGroups = _getAvailableGroups.Invoke(_frameGroupsSequenceIndex);

            if(availableGroups.Length == 0)
                return;

            var newIndex = _groupsSequenceProperty.arraySize;
            _groupsSequenceProperty.InsertArrayElementAtIndex(newIndex);

            var newProperty = _groupsSequenceProperty.GetArrayElementAtIndex(newIndex);
            newProperty.stringValue = availableGroups[0];

            newProperty.serializedObject.ApplyModifiedProperties();
        }
    }
}
