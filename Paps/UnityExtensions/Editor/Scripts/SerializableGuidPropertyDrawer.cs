using UnityEditor;
using UnityEngine.UIElements;
using System;

namespace Paps.UnityExtensions.Editor
{
    [CustomPropertyDrawer(typeof(SerializableGuid))]
    public class SerializableGuidPropertyDrawer : PropertyDrawer, IDisposable
    {
        private TextField _textField;
        private IVisualElementScheduledItem _scheduledItem;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            _textField = new TextField(property.displayName)
            {
                isReadOnly = true,
                enabledSelf = false
            };

            _scheduledItem = _textField.schedule.Execute(() =>
            {
                try
                {
                    _textField.SetValueWithoutNotify(GetGuidString(property));
                }
                catch
                {
                    
                }
            }).Every(100);

            return _textField;
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

        public void Dispose()
        {
            _scheduledItem.Pause();
        }
    }
}
