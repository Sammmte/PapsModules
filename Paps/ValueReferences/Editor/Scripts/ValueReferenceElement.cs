using System;
using UnityEngine.UIElements;
using EditorObject = UnityEditor.Editor;
using System.Reflection;
using UnityEditor;

namespace Paps.ValueReferences.Editor
{
    [UxmlElement]
    public partial class ValueReferenceElement : VisualElement, IDisposable
    {
        private ValueReferenceAsset _valueReferenceAsset;
        private VisualElement _editorContainer;
        private Label _nameLabel;
        private Label _typeNameLabel;
        private Button _renameButton;
        private TextField _renameTextField;

        private EditorObject _editor;

        public void Initialize(ValueReferenceAsset valueReferenceAsset)
        {
            _valueReferenceAsset = valueReferenceAsset;
            _editor = EditorObject.CreateEditor(_valueReferenceAsset);

            _editorContainer = this.Q("EditorContainer");
            _nameLabel = this.Q<Label>("NameLabel");
            _typeNameLabel = this.Q<Label>("TypeNameLabel");
            _renameButton = this.Q<Button>("RenameButton");
            _renameTextField = this.Q<TextField>("RenameTextField");

            RefreshName();

            _renameButton.clicked += OnRenameButtonClicked;
            _renameTextField.RegisterCallback<ChangeEvent<string>>(ev =>
            {
                Rename(ev.newValue);
            });

            _renameTextField.RegisterCallback<FocusOutEvent>(ev =>
            {
                HideRenameView();
            });

            var valueProperty = valueReferenceAsset.GetType().GetProperty("Value", BindingFlags.Public | BindingFlags.Instance);

            _typeNameLabel.text = valueProperty.PropertyType.Name;

            _editorContainer.Add(_editor.CreateInspectorGUI());
        }

        private void OnRenameButtonClicked()
        {
            ShowRenameView();
        }

        private void ShowRenameView()
        {
            _renameTextField.style.display = DisplayStyle.Flex;
            _renameTextField.SetValueWithoutNotify(_valueReferenceAsset.name);
            _renameTextField.Focus();

            _nameLabel.style.display = DisplayStyle.None;
        }

        private void HideRenameView()
        {
            _nameLabel.style.display = DisplayStyle.Flex;

            _renameTextField.style.display = DisplayStyle.None;
        }

        private void Rename(string newName)
        {
            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(_valueReferenceAsset), newName);

            RefreshName();
        }

        private void RefreshName()
        {
            _nameLabel.text = _valueReferenceAsset.name;
            _nameLabel.tooltip = _valueReferenceAsset.name;
        }

        public void Dispose()
        {
            EditorObject.DestroyImmediate(_editor);
        }
    }
}
