using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Paps.Optionals.Editor
{
    [CustomPropertyDrawer(typeof(Optional<>))]
    public class OptionalPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var valueProperty = property.FindPropertyRelative("_value");
            var serializedByUnityFlagProperty = property.FindPropertyRelative("_serializedByUnityFlag");
            var considerItHasValueProperty = property.FindPropertyRelative("_considerItHasValue");
            
            var container = new VisualElement();
            
            var valueField = new PropertyField(valueProperty);
            var considerItHasValueField = new PropertyField(considerItHasValueProperty);
            var foldout = new Foldout();
            foldout.text = property.displayName;
            
            container.Add(foldout);
            foldout.Add(considerItHasValueField);
            foldout.Add(valueField);
            
            serializedByUnityFlagProperty.boolValue = true;
            serializedByUnityFlagProperty.serializedObject.ApplyModifiedProperties();

            return container;
        }
    }
}
