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
        private Func<int, UpdateSchemaGroup[]> _getAvailableGroups;
        private Func<HashSet<UpdateSchemaGroup>> _getGroups;

        private SerializedProperty _property;

        public void Initialize(Func<int, UpdateSchemaGroup[]> getAvailableGroupsForSequence, Func<HashSet<UpdateSchemaGroup>> getGroups)
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

            var menuGroups = availableGroups.ToList();
            if (currentGroup != null)
                menuGroups.Add(currentGroup);
            
            menuGroups = menuGroups.OrderBy(group => group.name).ToList();

            var genericMenu = new GenericMenu();

            for(int i = 0; i < menuGroups.Count; i++)
            {
                var group = menuGroups[i];

                if(group == currentGroup)
                {
                    genericMenu.AddItem(new GUIContent(group.name), true, () => { });
                }
                else
                {
                    genericMenu.AddItem(new GUIContent(group.name), false, OnGroupSelected, group);
                }
            }

            genericMenu.ShowAsContext();
        }

        private void OnGroupSelected(object groupObj)
        {
            var group = (UpdateSchemaGroup)groupObj;

            _property.objectReferenceValue = group;
            _property.serializedObject.ApplyModifiedProperties();

            _groupButton.text = GetCurrentGroupName();
        }

        private string GetCurrentGroupName()
        {
            var group = _property.objectReferenceValue as UpdateSchemaGroup;
            
            if(group != null)
            {
                return group.name;
            }

            return "NO_GROUP_NAME";
        }

        private UpdateSchemaGroup GetCurrentGroup()
        {
            return _property.objectReferenceValue as UpdateSchemaGroup;
        }
    }
}
