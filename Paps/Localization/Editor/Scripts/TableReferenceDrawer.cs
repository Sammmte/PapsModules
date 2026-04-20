using Paps.UnityExtensions;
using Paps.UnityExtensions.Editor;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.Localization;
using UnityEngine;
using UnityEngine.UIElements;

namespace Paps.Localization.Editor
{
    [CustomPropertyDrawer(typeof(TableReference))]
    public class TableReferenceDrawer : PropertyDrawer
    {
        private bool _selectSourceButtonClicked;

        private SerializedProperty _tableIdProperty;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            _tableIdProperty = property.FindPropertyRelativeBakingField(nameof(TableReference.TableId));

            var container = LocalizationEditorConfiguration.Instance.LocalizationFieldTreeAsset.CloneTree();

            var label = container.Q<Label>("FieldLabel");
            var buttonContainer = container.Q("DropdownButtonIMGUIContainer");

            label.text = property.displayName;
            buttonContainer.Add(new IMGUIContainer(() => DrawTableSourceButton()));

            return container;
        }

        private void DrawTableSourceButton()
        {
            GUILayout.BeginVertical();

            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();

            if (GUILayout.Button(GetSelectSourceButtonStringState()))
            {
                _selectSourceButtonClicked = true;
            }
            
            GUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();

            GUILayout.EndVertical();

            if(_selectSourceButtonClicked && Event.current.type == EventType.Repaint)
            {
                ShowDropdown();
                _selectSourceButtonClicked = false;
            }
        }

        private string[] GetTableIds()
        {
            return LocalizationEditorSettings.GetStringTableCollections().Select(c => c.name).ToArray();
        }

        private void ShowDropdown()
        {
            var rect = GUILayoutUtility.GetLastRect();

            var advancedDropdown = new LocalizationTableAdvancedDropdown(new AdvancedDropdownState(), GetTableIds());
            advancedDropdown.OnItemSelected += OnItemSelected;
            advancedDropdown.Show(rect);
        }

        private string GetSelectSourceButtonStringState()
        {
            if(string.IsNullOrEmpty(_tableIdProperty.stringValue))
            {
                return "No Table Selected";
            }

            return _tableIdProperty.stringValue;
        }

        private void OnItemSelected(string tableId)
        {
            _tableIdProperty.stringValue = tableId;

            _tableIdProperty.serializedObject.ApplyModifiedProperties();
        }
    }
}
