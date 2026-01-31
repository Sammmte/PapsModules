using System;
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

        private SerializedProperty _property;

        public string CurrentGroup => _property.stringValue;

        public void Initialize(Func<int, string[]> getAvailableGroups)
        {
            _getAvailableGroups = getAvailableGroups;

            _groupButton = this.Q<Button>();
            _groupButton.clicked += OnButtonClicked;
        }

        public void SetData(SerializedProperty property, int frameGroupSequenceIndex)
        {
            CleanUp();

            _property = property;
            _frameGroupsSequenceIndex = frameGroupSequenceIndex;

            _groupButton.text = _property.stringValue;
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

            var menuGroups = availableGroups.Append(CurrentGroup).OrderBy(g => g).ToArray();

            var genericMenu = new GenericMenu();

            for(int i = 0; i < menuGroups.Length; i++)
            {
                var group = menuGroups[i];

                if(group == CurrentGroup)
                {
                    genericMenu.AddItem(new GUIContent(group), true, () => { });
                }
                else
                {
                    genericMenu.AddItem(new GUIContent(group), false, OnGroupSelected, group);
                }
            }

            genericMenu.ShowAsContext();
        }

        private void OnGroupSelected(object groupObj)
        {
            var group = groupObj as string;

            _property.stringValue = group;
            _property.serializedObject.ApplyModifiedProperties();

            _groupButton.text = group;
        }
    }
}
