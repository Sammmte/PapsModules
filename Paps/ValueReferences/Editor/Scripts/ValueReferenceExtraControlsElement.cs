using System;
using UnityEditor;
using UnityEngine.UIElements;

namespace Paps.ValueReferences.Editor
{
    [UxmlElement]
    public partial class ValueReferenceExtraControlsElement : VisualElement
    {
        private Button _deleteButton;
        private Button _pingButton;

        private ValueReferenceGroupAsset _currentGroup;
        private ValueReferenceAsset _currentValueReferenceAsset;

        private Action<ValueReferenceAsset> _onElementRemoved;
        
        public void Initialize(Action<ValueReferenceAsset> onElementRemoved)
        {
            _onElementRemoved = onElementRemoved;

            _deleteButton = this.Q<Button>("DeleteButton");
            _pingButton = this.Q<Button>("PingButton");

            _deleteButton.clicked += ShowRemoveElementDialog;
            _pingButton.clicked += PingAsset;
        }

        public void SetData(ValueReferenceAsset valueReferenceAsset, ValueReferenceGroupAsset groupAsset)
        {
            CleanUp();

            _currentValueReferenceAsset = valueReferenceAsset;
            _currentGroup = groupAsset;
        }

        private void PingAsset()
        {
            EditorGUIUtility.PingObject(_currentValueReferenceAsset);
        }

        private void ShowRemoveElementDialog()
        {
            if(EditorUtility.DisplayDialog(
                "Remove asset from group", 
                $"HEADS UP! This will NOT delete the item, only remove it from {_currentGroup.name} group.\nDo you wish to continue?",
                "Accept", "Cancel"))
            {
                RemoveAssetFromGroup();
            }
        }

        private void RemoveAssetFromGroup()
        {
            RemoveItem(_currentValueReferenceAsset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            _onElementRemoved(_currentValueReferenceAsset);
        }

        private void RemoveItem(ValueReferenceAsset element)
        {
            var serializedObject = new SerializedObject(_currentGroup);
            var valueReferencesArrayProperty = serializedObject.FindProperty(nameof(ValueReferenceGroupAsset.ValueReferenceAssets));

            serializedObject.Update();

            var index = IndexOf(element);

            valueReferencesArrayProperty.DeleteArrayElementAtIndex(index);

            serializedObject.ApplyModifiedProperties();
        }

        private int IndexOf(ValueReferenceAsset asset)
        {
            for(int i = 0; i < _currentGroup.ValueReferenceAssets.Length; i++)
            {
                if(_currentGroup.ValueReferenceAssets[i] == asset)
                    return i;
            }

            return -1;
        }

        public void CleanUp()
        {
            _currentGroup = null;
            _currentValueReferenceAsset = null;
        }
    }
}
