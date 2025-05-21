using System.Linq;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Paps.Physics.Editor
{
    public class PhysicsBuildPreprocess : IPreprocessBuildWithReport
    {
        public int callbackOrder { get; }
        public void OnPreprocessBuild(BuildReport report)
        {
            EnsureNamedLayerMasksConfigurationIsPreloaded(NamedLayerMaskConfiguration.Instance);
        }
        
        private static void EnsureNamedLayerMasksConfigurationIsPreloaded(NamedLayerMaskConfiguration asset)
        {
            var preloadedAssets = UnityEditor.PlayerSettings.GetPreloadedAssets();

            if (!preloadedAssets.Contains(asset))
            {
                var newPreloaded = preloadedAssets.Append(asset);
                UnityEditor.PlayerSettings.SetPreloadedAssets(newPreloaded.ToArray());
            }
        }
    }
}
