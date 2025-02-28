using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Paps.Build
{
    public class BuildSettings
    {
        public string OutputPath { get; set; }
        public BuildTarget BuildTarget { get; set; } = BuildTarget.StandaloneWindows64;
        public BuildOptions BuildOptions { get; set; } = BuildOptions.None;

        private List<string> _scenePaths = new List<string>();
        private List<string> _defineSymbols = new List<string>();
        private List<string> _dontIncludeAddressablesGroups = new List<string>();
        private List<object> _settings = new List<object>();

        public T GetCustomSettings<T>()
        {
            return (T)_settings.First(o => o is T);
        }

        public object[] GetCustomSettings()
        {
            return _settings.ToArray();
        }

        public void AddSettings(object settings)
        {
            _settings.Add(settings);
        }

        public void RemoveSettings(object settings)
        {
            _settings.Remove(settings);
        }

        public void AddDefineSymbol(string defineSymbol)
        {
            _defineSymbols.Add(defineSymbol);
        }

        public void RemoveDefineSymbol(string defineSymbol)
        {
            _defineSymbols.Remove(defineSymbol);
        }

        public string[] GetDefineSymbols()
        {
            return _defineSymbols.ToArray();
        }

        public void AddScenePath(string scenePath)
        {
            _scenePaths.Add(scenePath);
        }

        public void RemoveScenePath(string scenePath)
        {
            _scenePaths.Remove(scenePath);
        }

        public string[] GetScenePaths()
        {
            return _scenePaths.ToArray();
        }

        public void SetScenePaths(string[] scenePaths)
        {
            _scenePaths.Clear();
            _scenePaths.AddRange(scenePaths);
        }

        public void DontIncludeAddressablesGroup(string group)
        {
            _dontIncludeAddressablesGroups.Add(group);
        }

        public string[] GetNotIncludedAddressablesGroups() => _dontIncludeAddressablesGroups.ToArray();
    }
}