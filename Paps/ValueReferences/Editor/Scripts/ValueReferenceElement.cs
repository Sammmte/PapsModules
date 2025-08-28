using System;
using UnityEngine.UIElements;

namespace Paps.ValueReferences.Editor
{
    [UxmlElement]
    public partial class ValueReferenceElement : VisualElement
    {
        public ValueReferenceAsset ValueReferenceAsset { get; private set; }
        public int Index { get; private set; }

        private UnityEditor.Editor _assetEditor;

        public event Action<ValueReferenceElement> OnDeleteRequested;
        
        public void Initialize(ValueReferenceAsset valueReferenceAsset, int index)
        {
            ValueReferenceAsset = valueReferenceAsset;
            Index = index;
            
            _assetEditor = UnityEditor.Editor.CreateEditor(ValueReferenceAsset);

            var assetNameField = this.Q<TextField>("AssetNameField");
            var editorContainer = this.Q("EditorContainer");
            var deleteButton = this.Q<Button>("DeleteButton");

            assetNameField.value = ValueReferenceAsset.name;
            assetNameField.RegisterValueChangedCallback(ev =>
            {
                if (!string.IsNullOrEmpty(ev.newValue))
                {
                    ValueReferenceAsset.name = ev.newValue;
                }
                
                assetNameField.SetValueWithoutNotify(ValueReferenceAsset.name);
            });
            
            editorContainer.Add(_assetEditor.CreateInspectorGUI());

            deleteButton.clicked += NotifyDeleteRequested;
        }

        private void NotifyDeleteRequested()
        {
            OnDeleteRequested?.Invoke(this);
        }
    }
}