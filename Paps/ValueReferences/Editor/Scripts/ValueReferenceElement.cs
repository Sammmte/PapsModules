using System;
using UnityEngine.UIElements;
using EditorObject = UnityEditor.Editor;
using System.Reflection;

namespace Paps.ValueReferences.Editor
{
    [UxmlElement]
    public partial class ValueReferenceElement : VisualElement, IDisposable
    {
        private ValueReferenceAsset _valueReferenceAsset;
        private VisualElement _editorContainer;
        private Label _nameLabel;
        private Label _typeNameLabel;

        private EditorObject _editor;

        public void Initialize(ValueReferenceAsset valueReferenceAsset)
        {
            _valueReferenceAsset = valueReferenceAsset;
            _editor = EditorObject.CreateEditor(_valueReferenceAsset);

            _editorContainer = this.Q("EditorContainer");
            _nameLabel = this.Q<Label>("NameLabel");
            _typeNameLabel = this.Q<Label>("TypeNameLabel");

            _nameLabel.text = valueReferenceAsset.name;
            _nameLabel.tooltip = valueReferenceAsset.name;

            var valueProperty = valueReferenceAsset.GetType().GetProperty("Value", BindingFlags.Public | BindingFlags.Instance);

            _typeNameLabel.text = valueProperty.PropertyType.Name;

            _editorContainer.Add(_editor.CreateInspectorGUI());
        }

        public void Dispose()
        {
            EditorObject.DestroyImmediate(_editor);
        }
    }
}
