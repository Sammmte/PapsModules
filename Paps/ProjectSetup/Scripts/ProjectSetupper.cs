using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Paps.ProjectSetup
{
    public static class ProjectSetupper
    {
        public static void SetupProject(object[] customProjectSetupSettingsObjects)
        {
            var projectSetupSettingsHandlers = TypeCache.GetTypesDerivedFrom<IProjectSetupSettingsHandler>()
                .Select(t => (IProjectSetupSettingsHandler)Activator.CreateInstance(t))
                .Where(i => customProjectSetupSettingsObjects.Any(s => s.GetType() == i.SettingsType))
                .OrderBy(i => i.Order);

            foreach (var handler in projectSetupSettingsHandlers)
            {
                var customSettings = customProjectSetupSettingsObjects.First(s => s.GetType() == handler.SettingsType);
                handler.HandleSettings(customSettings);
            }

            Debug.Log("Project setupped!");
        }
    }
}