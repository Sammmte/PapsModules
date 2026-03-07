using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Paps.Update.Editor
{
    [UxmlElement]
    public partial class FrameGroupsSequenceItemElement : VisualElement
    {
        private Button _groupButton;
        private int _frameGroupsSequenceIndex;
        private Func<int, string[]> _getAvailableGroups;
        private Func<HashSet<string>> _getGroups;

        private SerializedProperty _property;

        public void Initialize(Func<int, string[]> getAvailableGroupsForSequence, Func<HashSet<string>> getGroups)
        {
            _getAvailableGroups = getAvailableGroupsForSequence;
            _getGroups = getGroups;

            _groupButton = this.Q<Button>();
            _groupButton.clicked += OnButtonClicked;
        }

        public void SetData(SerializedProperty property, int frameGroupSequenceIndex)
        {
            CleanUp();

            _property = property;
            _frameGroupsSequenceIndex = frameGroupSequenceIndex;

            _groupButton.text = GetCurrentGroupName();
        }

        public void CleanUp()
        {
            _frameGroupsSequenceIndex = -1;
            _property = null;
            _groupButton.text = string.Empty;
        }

        private void OnButtonClicked()
        {
            var availableGroups = _getAvailableGroups.Invoke(_frameGroupsSequenceIndex);

            if(availableGroups.Length == 0)
                return;

            var currentGroup = GetCurrentGroup();

            var menuGroups = availableGroups.Append(currentGroup).OrderBy(group => group).ToArray();

            var genericMenu = new GenericMenu();

            for(int i = 0; i < menuGroups.Length; i++)
            {
                var group = menuGroups[i];

                if(group.Equals(currentGroup))
                {
                    genericMenu.AddItem(new GUIContent(group), true, () => { });
                }
                else
                {
                    genericMenu.AddItem(new GUIContent(group), false, OnGroupSelected, UpdateSchemaUtils.GetIdOfGroup(group));
                }
            }

            genericMenu.ShowAsContext();
        }

        private void OnGroupSelected(object groupObj)
        {
            var group = (int)groupObj;

            _property.intValue = group;
            _property.serializedObject.ApplyModifiedProperties();

            _groupButton.text = GetCurrentGroupName();
        }

        private string GetCurrentGroupName()
        {
            if(UpdateSchemaUtils.TryGetGroupById(_property.intValue, _getGroups().ToList(), out var group))
            {
                return group;
            }

            return "NO_GROUP_NAME";
        }

        private string GetCurrentGroup()
        {
            if(UpdateSchemaUtils.TryGetGroupById(_property.intValue, _getGroups().ToList(), out var group))
            {
                return group;
            }

            throw new InvalidOperationException("Invalid group");
        }
    }
}
