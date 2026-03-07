using UnityEngine;

namespace Paps.Levels
{
    public static class LevelSetupExtensions
    {
        public static T AddLevelBoundComponent<T>(this GameObject gameObject) where T : Component, ILevelBound
        {
            return LevelManager.Instance.AddLevelBoundComponent<T>(gameObject);
        }

        public static bool DidLoaded(this ILevelBound levelBound) => LevelManager.Instance.DidLoaded(levelBound);

        public static bool DidKickstart(this ILevelBound levelBound) => LevelManager.Instance.DidKickstart(levelBound);
    }
}
