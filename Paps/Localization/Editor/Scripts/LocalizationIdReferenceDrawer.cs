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
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var tableIdProperty = property.FindPropertyRelativeBakingField(nameof(LocalizationIdReference.TableId)).FindPropertyRelativeBakingField(nameof(TableReference.TableId));
            var localizationIdProperty = property.FindPropertyRelativeBakingField(nameof(LocalizationIdReference.LocalizationId));

            var container = LocalizationEditorConfiguration.Instance.LocalizationFieldTreeAsset.CloneTree();

            var label = container.Q<Label>("FieldLabel");
            var buttonContainer = container.Q("DropdownButtonIMGUIContainer");

            label.text = property.displayName;

            var selectSourceButtonClicked = false;

            buttonContainer.Add(new IMGUIContainer(() =>
            {
                GUILayout.BeginVertical();

                GUILayout.FlexibleSpace();

                GUILayout.BeginHorizontal();

                if (GUILayout.Button(GetSelectSourceButtonStringState()))
                {
                    selectSourceButtonClicked = true;
                }

                GUILayout.EndHorizontal();

                GUILayout.FlexibleSpace();

                GUILayout.EndVertical();

                if(selectSourceButtonClicked && Event.current.type == EventType.Repaint)
                {
                    ShowDropdown();
                    selectSourceButtonClicked = false;
                }
            }));

            return container;

            void ShowDropdown()
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

            string GetSelectSourceButtonStringState()
            {
                if(string.IsNullOrEmpty(localizationIdProperty.stringValue) || string.IsNullOrEmpty(tableIdProperty.stringValue))
                {
                    return "No Localization Key Selected";
                }

                return $"{tableIdProperty.stringValue}/{localizationIdProperty.stringValue}";
            }

            void OnItemSelected(LocalizationIdReference localizationId)
            {
                localizationIdProperty.stringValue = localizationId.LocalizationId;
                tableIdProperty.stringValue = localizationId.TableId;

                localizationIdProperty.serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
