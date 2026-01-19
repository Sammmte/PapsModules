using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Reflection;

namespace Paps.ValueReferences.Editor
{
    [UxmlElement]
    public partial class ValueReferenceGroupAddButton : VisualElement
    {
        private Button _button;

        private GenericMenu _addValueReferenceAssetMenu;

        private ValueReferenceGroupAsset _groupAsset;
        private Action<ValueReferenceAsset> _onNewItemAdded;

        public void Initialize(ValueReferenceGroupAsset groupAsset, Action<ValueReferenceAsset> onNewItemAdded)
        {
            _groupAsset = groupAsset;
            _onNewItemAdded = onNewItemAdded;

            _button = this.Q<Button>();
            _button.clicked += ShowAddContextMenu;

            _addValueReferenceAssetMenu = new GenericMenu();
            
            var createAssetMenuAttributesPerType = ValueReferencesEditorManager.GetCreateAssetMenuPerType();

            foreach(var tuple in createAssetMenuAttributesPerType)
            {
                _addValueReferenceAssetMenu.AddItem(new GUIContent(tuple.Attribute.menuName), 
                    false, AddItemOfType, tuple.Type);
            }
        }

        private void ShowAddContextMenu()
        {
            _addValueReferenceAssetMenu.ShowAsContext();
        }

        private void AddItemOfType(object valueReferenceType)
        {
            var type = valueReferenceType as Type;

            var newAsset = ScriptableObject.CreateInstance(type) as ValueReferenceAsset;

            var thisGroupPath = AssetDatabase.GetAssetPath(_groupAsset);

            var folderPath = Path.GetDirectoryName(thisGroupPath);

            var genericType = type.GetProperty("Value", BindingFlags.Public | BindingFlags.Instance).PropertyType;

            AssetDatabase.CreateAsset(newAsset, Path.Combine(folderPath, $"New{genericType.Name}ValueReference.asset"));

            AddItem(newAsset);

            _onNewItemAdded(newAsset);
        }

        private void AddItem(ValueReferenceAsset item)
        {
            var serializedObject = new SerializedObject(_groupAsset);
            var valueReferencesArrayProperty = serializedObject.FindProperty(nameof(_groupAsset.ValueReferenceAssets));

            var newIndex = valueReferencesArrayProperty.arraySize;

            valueReferencesArrayProperty.InsertArrayElementAtIndex(newIndex);

            var newProperty = valueReferencesArrayProperty.GetArrayElementAtIndex(newIndex);

            newProperty.objectReferenceValue = item;

            serializedObject.ApplyModifiedProperties();

            serializedObject.Dispose();
        }
    }
}
