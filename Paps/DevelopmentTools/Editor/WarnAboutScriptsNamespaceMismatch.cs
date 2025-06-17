using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace Paps.DevelopmentTools.Editor
{
    [InitializeOnLoad]
    public static class WarnAboutScriptsNamespaceMismatch
    {
        private struct ScriptInfo
        {
            public string Path;
            public string Namespace;
        }

        private const string PATH = "Assets/Game";
        private const string GLOBAL_NAMESPACE = "Global";
        private const string DEFAULT_ASSEMBLY = "Assembly-CSharp.dll";
        private const string DEFAULT_EDITOR_ASSEMBLY = "Assembly-CSharp-Editor.dll";

        static WarnAboutScriptsNamespaceMismatch()
        {
            WarnForEachMismatch();
        }

        private static ScriptInfo[] GetEligibleScripts()
        {
            return AssetDatabase.FindAssets("t:script", new[] { PATH })
                .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                .Select(assetPath => new ScriptInfo() { Path = assetPath, Namespace = NamespaceOf(AssetDatabase.LoadAssetAtPath<MonoScript>(assetPath)) })
                .ToArray();
        }

        private static void WarnForEachMismatch()
        {
            var scripts = GetEligibleScripts();

            foreach(var script in scripts)
            {
                var expectedNamespace = CompilationPipeline.GetAssemblyRootNamespaceFromScriptPath(script.Path);

                if(expectedNamespace == string.Empty)
                {
                    var assemblyName = CompilationPipeline.GetAssemblyNameFromScriptPath(script.Path);
                    if (AssemblyIsUnityDefault(assemblyName))
                        Debug.LogWarning($"Script {script.Path} has no assembly definition");
                    else
                        Debug.LogWarning($"Assembly Definition of name {assemblyName} has no root namespace. Script of reference is {script.Path}");
                    continue;
                }

                if (!script.Namespace.StartsWith(expectedNamespace))
                    Debug.LogWarning($"Script with path {script.Path} should be on namespace {expectedNamespace} but is on namespace {script.Namespace}");
            }
        }

        private static bool AssemblyIsUnityDefault(string assemblyName)
        {
            return assemblyName == DEFAULT_ASSEMBLY || assemblyName == DEFAULT_EDITOR_ASSEMBLY;
        }

        private static string NamespaceOf(MonoScript monoScript)
        {
            var lines = monoScript.text.Split(new[] { '\r', '\n' });

            var namespaceLine = lines.FirstOrDefault(line => line.Contains("namespace"));

            if (namespaceLine == null)
                return GLOBAL_NAMESPACE;

            var scriptNamespace = new StringBuilder(namespaceLine);

            scriptNamespace.Remove(0, "namespace ".Length);

            var stripCharactersIndexes = new List<int>();

            for(int i = 0; i < scriptNamespace.Length; i++)
            {
                var currentCharacter = scriptNamespace[i];

                if (!char.IsLetter(currentCharacter) && currentCharacter != '.')
                    stripCharactersIndexes.Add(i);
            }

            foreach(var index in stripCharactersIndexes)
            {
                scriptNamespace.Remove(index, 1);
            }

            return scriptNamespace.ToString();
        }
    }
}