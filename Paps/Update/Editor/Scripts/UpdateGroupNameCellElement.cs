using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using System.Collections.Generic;

namespace Paps.Update.Editor
{
    [UxmlElement]
    public partial class UpdateGroupNameCellElement : VisualElement
    {
        private Label _nameLabel;
        private TextField _renameField;
        private Button _renameButton;

        private Func<HashSet<string>> _getGroups;

        public SerializedProperty SerializedProperty { get; private set; }

        public void Initialize(Func<HashSet<string>> getGroups)
        {
            _getGroups = getGroups;

            _nameLabel = this.Q<Label>("NameLabel");
            _renameField = this.Q<TextField>("RenameField");
            _renameButton = this.Q<Button>("RenameButton");

            _renameButton.clicked += ShowRenameView;
            _renameField.RegisterCallback<FocusOutEvent>(ev => HideRenameView());
            _renameField.RegisterValueChangedCallback(ev =>
            {
                ApplyRename(ev.newValue);
                HideRenameView();
            });
        }

        public void SetData(SerializedProperty property)
        {
            CleanUp();

            SerializedProperty = property;

            _nameLabel.BindProperty(property);
        }

        public void ShowRenameView()
        {
            ShowRenameView(_nameLabel.text);
        }
        
        public void ShowRenameView(string onShowName)
        {
            SerializedProperty.serializedObject.Update();

            _nameLabel.style.display = DisplayStyle.None;
            _renameField.style.display = DisplayStyle.Flex;

            _renameField.SetValueWithoutNotify(onShowName);
            _renameField.Focus();
        }

        public void HideRenameView()
        {
            _nameLabel.style.display = DisplayStyle.Flex;
            _renameField.style.display = DisplayStyle.None;
        }

        private void ApplyRename(string newName)
        {
            if(!IsValidRename(newName))
                return;

            SerializedProperty.stringValue = newName.Trim();
            SerializedProperty.serializedObject.ApplyModifiedProperties();
        }

        private bool IsValidRename(string newName)
        {
            return !(string.IsNullOrEmpty(newName) || _getGroups.Invoke().Contains(newName));
        }

        public void CleanUp()
        {
            HideRenameView();

            _nameLabel.Unbind();

            SerializedProperty = null;
        }
    }
}
