using UnityEditor;

namespace Paps.UnityExtensions.Editor
{
    public static class SerializedObjectExtensions
    {
        public static SerializedProperty FindPropertyBakingField(this SerializedObject serializedObject, string propertyName)
        {
            return serializedObject.FindProperty($"<{propertyName}>k__BackingField");
        }

        public static SerializedProperty FindPropertyRelativeBakingField(this SerializedProperty serializedProperty,
            string relativePropertyName)
        {
            return serializedProperty.FindPropertyRelative($"<{relativePropertyName}>k__BackingField");
        }
    }
}
