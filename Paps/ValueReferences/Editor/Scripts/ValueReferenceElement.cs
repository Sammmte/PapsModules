using System;
using UnityEngine.UIElements;
using EditorObject = UnityEditor.Editor;
using System.Reflection;
using UnityEditor;
using Paps.Optionals;

namespace Paps.ValueReferences.Editor
{
    [UxmlElement]
    public partial class ValueReferenceElement : VisualElement, IDisposable
    {
        public struct Options
        {
            public bool HideDelete;
        }

        private ValueReferenceAsset _valueReferenceAsset;
        private VisualElement _editorContainer;
        private Label _nameLabel;
        private Label _typeNameLabel;
        private Button _renameButton;
        private TextField _renameTextField;
        private Button _deleteButton;
        private Button _pingButton;

        private EditorObject _editor;

        public event Action<ValueReferenceElement> OnDeleteRequested;

        public ValueReferenceAsset ValueReferenceAsset => _valueReferenceAsset;

        public void Initialize(ValueReferenceAsset valueReferenceAsset, Optional<Options> options = default)
        {
            _valueReferenceAsset = valueReferenceAsset;
            _editor = EditorObject.CreateEditor(_valueReferenceAsset);

            _editorContainer = this.Q("EditorContainer");
            _nameLabel = this.Q<Label>("NameLabel");
            _typeNameLabel = this.Q<Label>("TypeNameLabel");
            _renameButton = this.Q<Button>("RenameButton");
            _renameTextField = this.Q<TextField>("RenameTextField");
            _deleteButton = this.Q<Button>("DeleteButton");
            _pingButton = this.Q<Button>("PingButton");

            RefreshName();

            _renameButton.clicked += OnRenameButtonClicked;
            _renameTextField.RegisterCallback<ChangeEvent<string>>(ev =>
            {
                Rename(ev.newValue);
                HideRenameView();
            });

            _renameTextField.RegisterCallback<FocusOutEvent>(ev =>
            {
                HideRenameView();
            });

            _deleteButton.clicked += NotifyDeleteRequested;
            _pingButton.clicked += PingAsset;

            var valueProperty = valueReferenceAsset.GetType().GetProperty("Value", BindingFlags.Public | BindingFlags.Instance);

            _typeNameLabel.text = valueProperty.PropertyType.Name;

            _editorContainer.Add(_editor.CreateInspectorGUI());

            if(options.HasValue)
                ApplyOptions(options);
        }

        private void PingAsset()
        {
            EditorGUIUtility.PingObject(_valueReferenceAsset);
        }

        private void NotifyDeleteRequested()
        {
            OnDeleteRequested?.Invoke(this);
        }

        private void OnRenameButtonClicked()
        {
            ShowRenameView();
        }

        public void ShowRenameView()
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
            newName = newName.Trim();

            if(_valueReferenceAsset.name == newName)
                return;

            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(_valueReferenceAsset), newName);

            RefreshName();
        }

        private void RefreshName()
        {
            _nameLabel.text = _valueReferenceAsset.name;
            _nameLabel.tooltip = _valueReferenceAsset.name;
        }

        public void ApplyOptions(Options options)
        {
            if(options.HideDelete)
            {
                _deleteButton.style.display = DisplayStyle.None;
                _deleteButton.enabledSelf = false;
            }
            else
            {
                _deleteButton.style.display = DisplayStyle.Flex;
                _deleteButton.enabledSelf = true;
            }
        }
        public void Dispose()
        {
            EditorObject.DestroyImmediate(_editor);
        }
    }
}
