using System;
using UnityEngine;

namespace Paps.UnityExtensions
{
    public static class GameObjectExtensions
    {
        public static void DontDestroyOnLoad(this GameObject obj)
        {
            obj.transform.SetParent(null);
            GameObject.DontDestroyOnLoad(obj);
        }

        public static string GetUnityName<T>(this T unityObject) where T : class
        {
            return unityObject.AsUnityObject().name;
        }

        public static GameObject GetGameObject<T>(this T component) where T : class
        {
            return component.AsUnityComponent().gameObject;
        }

        public static TComponentResult GetComponent<TComponentExtended, TComponentResult>(this TComponentExtended extended) 
            where TComponentExtended : class where TComponentResult : class
        {
            return extended.AsUnityComponent().GetComponent<TComponentResult>();
        }

        public static bool TryGetComponent<TComponentExtended, TComponentResult>(this TComponentExtended extended, out TComponentResult result)
            where TComponentExtended : class where TComponentResult : class
        {
            return extended.AsUnityComponent().TryGetComponent(out result);
        }

        public static Component AsUnityComponent<T>(this T classOrInterfaceInstance) where T : class
        {
            if(classOrInterfaceInstance is Component component)
            {
                return component;
            }

            throw new InvalidOperationException($"Object of type {classOrInterfaceInstance.GetType().Name} is not a Unity component");
        }

        public static UnityEngine.Object AsUnityObject<T>(this T classOrInterafaceInstance) where T : class
        {
            if(classOrInterafaceInstance is UnityEngine.Object obj)
            {
                return obj;
            }

            throw new InvalidOperationException($"Object of type {classOrInterafaceInstance.GetType().Name} is not a Unity Object");
        }
    }
}
