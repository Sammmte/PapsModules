using System.Reflection;
using System;

namespace Paps.ReflectionExtensions
{
    public static class ReflectionObjectExtensions
    {
        public static void CallPrivate(this object obj, string methodName, object[] parameters = null)
        {
            var method = obj.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);

            method.Invoke(obj, parameters);
        }

        public static object CallStaticPrivate(Type type, string methodName, params object[] parameters)
        {
            var method = type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static);
            
            return method.Invoke(null, parameters);
        }

        public static void SetPrivateField<T>(this object obj, string fieldName, T value)
        {
            var field = obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);

            field.SetValue(obj, value);
        }

        public static T GetPrivateField<T>(this object obj, string fieldName)
        {
            return (T)obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(obj);
        }
    }
}