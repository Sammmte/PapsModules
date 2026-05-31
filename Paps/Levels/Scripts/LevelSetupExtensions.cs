using Paps.UnityExtensions;
using System.Threading;
using UnityEngine;

namespace Paps.Levels
{
    public static class LevelSetupExtensions
    {
        public static T AddLevelBoundComponent<T>(this GameObject gameObject) where T : Component, ILevelBound
        {
            return LevelManager.Instance.AddLevelBoundComponent<T>(gameObject);
        }

        public static bool IsLoading(this ILevelBound levelBound) => LevelManager.Instance.IsLoading(levelBound);
        public static bool DidLoad(this ILevelBound levelBound) => LevelManager.Instance.DidLoad(levelBound);
        public static bool IsSetupping(this ILevelBound levelBound) => LevelManager.Instance.IsSetupping(levelBound);
        public static bool DidSetup(this ILevelBound levelBound) => LevelManager.Instance.DidSetup(levelBound);
        public static bool IsKickstarting(this ILevelBound levelBound) => LevelManager.Instance.IsKickstarting(levelBound);
        public static bool DidKickstart(this ILevelBound levelBound) => LevelManager.Instance.DidKickstart(levelBound);
        public static bool IsUnloading(this ILevelBound levelBound) => LevelManager.Instance.IsUnloading(levelBound);
        public static bool DidUnload(this ILevelBound levelBound) => LevelManager.Instance.DidUnload(levelBound);
        public static CancellationToken GetUnloadCancellationToken(this ILevelBound levelBound) => LevelManager.Instance.GetUnloadCancellationToken(levelBound);
        public static string GetDebugName(this ILevelBound levelBound) => $"{levelBound.GetUnityName()}_Component:{levelBound.GetType().Name}";
    }
}
