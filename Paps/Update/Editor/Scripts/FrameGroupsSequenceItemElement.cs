using System;
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

            for(int i = 0; i < availableGroups.Length; i++)
            {
                Debug.Log($"AVAILABLE GROUP: {availableGroups[i]}");
            }
        }
    }
}
