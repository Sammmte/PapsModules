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
    [CustomPropertyDrawer(typeof(LocalizationIdReference))]
    public class LocalizationIdReferenceDrawer : PropertyDrawer
    {
        private bool _selectSourceButtonClicked;

        private SerializedProperty _tableIdProperty;
        private SerializedProperty _localizationIdProperty;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            _tableIdProperty = property.FindPropertyRelativeBakingField(nameof(LocalizationIdReference.TableId)).FindPropertyRelativeBakingField(nameof(TableReference.TableId));
            _localizationIdProperty = property.FindPropertyRelativeBakingField(nameof(LocalizationIdReference.LocalizationId));

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

        private void ShowDropdown()
        {
            var rect = GUILayoutUtility.GetLastRect();

            var advancedDropdown = new LocalizationIdAdvancedDropdown(new AdvancedDropdownState(), 
                LocalizationEditorSettings.GetStringTableCollections()
                    .ToDictionary(
                        c => c.name, 
                        c => c.SharedData.Entries.Select(e => e.Key).ToArray())
                );
            advancedDropdown.OnItemSelected += OnItemSelected;
            advancedDropdown.Show(rect);
        }

        private string GetSelectSourceButtonStringState()
        {
            if(string.IsNullOrEmpty(_localizationIdProperty.stringValue) || string.IsNullOrEmpty(_tableIdProperty.stringValue))
            {
                return "No Localization Key Selected";
            }

            return $"{_tableIdProperty.stringValue}/{_localizationIdProperty.stringValue}";
        }

        private void OnItemSelected(LocalizationIdReference localizationId)
        {
            _localizationIdProperty.stringValue = localizationId.LocalizationId;
            _tableIdProperty.stringValue = localizationId.TableId;

            _localizationIdProperty.serializedObject.ApplyModifiedProperties();
        }
    }
}
