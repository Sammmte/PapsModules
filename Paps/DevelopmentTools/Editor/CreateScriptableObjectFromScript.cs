using System.IO;
using UnityEditor;
using UnityEngine;
using System;

namespace Paps.DevelopmentTools.Editor
{
    public static class CreateScriptableObjectFromScript
    {
        private const string PATH = "Assets/Create/Create ScriptableObject From Script";

        [MenuItem(PATH, priority = -1100)]
        public static void CreateScriptableObjectFromSelectedScript()
        {
            var monoScript = Selection.activeObject as MonoScript;

            var type = monoScript.GetClass();

            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance(type), CreateNewScriptableObjectAssetPath(type));
        }

        private static string CreateNewScriptableObjectAssetPath(Type type)
        {
            return Path.Combine(GetCurrentDirectory(), type.Name + ".asset");
        }

        private static string GetCurrentDirectory()
        {
            return Path.GetDirectoryName(AssetDatabase.GetAssetPath(Selection.activeObject));
        }

        [MenuItem(PATH, validate = true)]
        public static bool Validate()
        {
            var selectedObject = Selection.activeObject;

            return selectedObject is MonoScript monoScript && IsScriptableObjectConcreteSubclass(monoScript);
        }

        private static bool IsScriptableObjectConcreteSubclass(MonoScript monoScript)
        {
            var scriptType = monoScript.GetClass();

            return scriptType != null && scriptType.IsSubclassOf(typeof(ScriptableObject)) && !scriptType.IsAbstract;
        }
    }
}
