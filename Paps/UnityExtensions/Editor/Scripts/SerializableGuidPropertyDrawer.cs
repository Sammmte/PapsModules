using UnityEditor;
using UnityEngine.UIElements;
using System;

namespace Paps.UnityExtensions.Editor
{
    [CustomPropertyDrawer(typeof(SerializableGuid))]
    public class SerializableGuidPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var textField = new TextField(property.displayName);

            textField.isReadOnly = true;
            textField.enabledSelf = false;

            textField.SetValueWithoutNotify(GetGuidString(property));

            return textField;
        }

        private string GetGuidString(SerializedProperty property)
        {
            return GuidFromSerializedProperty(property).ToString();
        }

        public static Guid GuidFromSerializedProperty(SerializedProperty prop)
        {
            return new Guid(prop.FindPropertyRelative("A").intValue, (short)prop.FindPropertyRelative("B").intValue, (short)prop.FindPropertyRelative("C").intValue,
                                   (byte)prop.FindPropertyRelative("D").intValue, (byte)prop.FindPropertyRelative("E").intValue, (byte)prop.FindPropertyRelative("F").intValue,
                                   (byte)prop.FindPropertyRelative("G").intValue, (byte)prop.FindPropertyRelative("H").intValue, (byte)prop.FindPropertyRelative("I").intValue,
                                   (byte)prop.FindPropertyRelative("J").intValue, (byte)prop.FindPropertyRelative("K").intValue);
        }
    }
}
