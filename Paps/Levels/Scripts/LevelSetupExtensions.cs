using UnityEngine;

namespace Paps.Levels
{
    public static class LevelSetupExtensions
    {
        public static T AddSetuppableComponent<T>(this GameObject gameObject) where T : Component, ILevelBound
        {
            return LevelManager.Instance.AddSetuppableComponent<T>(gameObject);
        }
    }
}
