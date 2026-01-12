using System;
using System.Linq;
using Unity.Serialization.Json;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditor.Build;
using UnityEngine;

namespace Paps.Build
{
    public static class Builder
    {
        public struct Options
        {
            public bool Simulacrum;
        }

        private static readonly Options DEFAULT_OPTIONS = new Options()
        {
            Simulacrum = false
        };

        public static void Build(BuildSettings buildSettings, Options? options = null)
        {
            var finalOptions = OptionsOrDefault(options);

            var allCustomBuildSettings = buildSettings.GetCustomSettings();
            var buildSettingsHandlers = TypeCache.GetTypesDerivedFrom<IBuildSettingsHandler>()
                .Select(t => (IBuildSettingsHandler)Activator.CreateInstance(t))
                .Where(i => allCustomBuildSettings.Any(s => s.GetType() == i.SettingsType))
                .OrderBy(i => i.Order);

            foreach(var buildSettingsHandler in buildSettingsHandlers)
            {
                var customSettings = allCustomBuildSettings.First(s => s.GetType() == buildSettingsHandler.SettingsType);
                buildSettingsHandler.HandleSettings(buildSettings, customSettings);
            }
            
            var buildPreprocessors = TypeCache.GetTypesDerivedFrom<IBuildPreprocessor>()
                .Select(t => (IBuildPreprocessor)Activator.CreateInstance(t))
                .OrderBy(i => i.Order);

            foreach (var preprocessor in buildPreprocessors)
            {
                preprocessor.Process(buildSettings);
            }

            var buildTargetGroup = BuildPipeline.GetBuildTargetGroup(buildSettings.BuildTarget);
            var namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup);
            var previousDefineSymbols = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget);
            PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, buildSettings.GetDefineSymbols());
            Debug.Log($"Build goes with define symbols: {PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget)}");
            var addressableGroupsNotIncluded = buildSettings.GetNotIncludedAddressablesGroups();
            SetAddressablesGroupsAsIncluded(addressableGroupsNotIncluded, false);
            Debug.Log($"Build removes addressables groups: {JsonSerialization.ToJson(addressableGroupsNotIncluded)}");
            Debug.Log($"Scenes included in build are {JsonSerialization.ToJson(buildSettings.GetScenePaths())}");

            if (!finalOptions.Simulacrum)
            {
                BuildPipeline.BuildPlayer(new BuildPlayerOptions()
                {
                    locationPathName = buildSettings.OutputPath,
                    options = buildSettings.BuildOptions,
                    scenes = buildSettings.GetScenePaths(),
                    target = buildSettings.BuildTarget,
                });
            }

            PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, previousDefineSymbols);
            SetAddressablesGroupsAsIncluded(buildSettings.GetNotIncludedAddressablesGroups(), true);
        }

        private static Options OptionsOrDefault(Options? options)
        {
            if(options == null)
                return DEFAULT_OPTIONS;

            return options.Value;
        }

        private static void SetAddressablesGroupsAsIncluded(string[] groups, bool included)
        {
            var schemas = AddressableAssetSettingsDefaultObject.Settings.groups
                .Where(g => groups.Contains(g.Name))
                .Select(g => g.Schemas
                .First(s => s is BundledAssetGroupSchema) as BundledAssetGroupSchema);

            foreach (var schema in schemas)
                schema.IncludeInBuild = included;
        }
    }
}