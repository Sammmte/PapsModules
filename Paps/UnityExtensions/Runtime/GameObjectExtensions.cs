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

        public static string GetUnityName<T>(this T component) where T : class
        {
            return component.AsUnityComponent().name;
        }

        public static GameObject GetGameObject<T>(this T component) where T : class
        {
            return component.AsUnityComponent().gameObject;
        }

        public static Component AsUnityComponent<T>(this T classOrInterfaceInstance) where T : class
        {
            if(classOrInterfaceInstance is Component component)
            {
                return component;
            }

            throw new InvalidOperationException($"Object of type {classOrInterfaceInstance.GetType().Name} is not a Unity component");
        }
    }
}
