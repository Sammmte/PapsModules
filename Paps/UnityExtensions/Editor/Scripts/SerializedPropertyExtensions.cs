using System.Reflection;
using UnityEditor;

namespace Paps.UnityExtensions.Editor
{
    public static class SerializedPropertyExtensions
    {
        public static object GetTargetObject(this SerializedProperty prop)
        {
            var targetObject = prop.serializedObject.targetObject;
            var path = prop.propertyPath.Replace(".Array.data[", "[");
            var elements = path.Split('.');
    
            object currentObject = targetObject;
            foreach (var element in elements)
            {
                if (element.Contains("["))
                {
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var indexString = element.Substring(element.IndexOf("[") + 1, element.IndexOf("]") - element.IndexOf("[") - 1);
                    var index = int.Parse(indexString);
                    currentObject = GetFieldValue(currentObject, elementName, index);
                }
                else
                {
                    currentObject = GetFieldValue(currentObject, element);
                }
            }
            return currentObject;
        }
    
        private static object GetFieldValue(object source, string fieldName, int index = -1)
        {
            if (source == null) return null;
            var type = source.GetType();
            var field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (field == null) return null;
            var value = field.GetValue(source);
            if (index != -1)
            {
                // Handle List<T> or Array
                var list = value as System.Collections.IList;
                return list != null && index < list.Count ? list[index] : null;
            }
            return value;
        }
    }
}
