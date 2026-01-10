using UnityEngine;

namespace Paps.LevelSetup
{
    public static class LevelSetupExtensions
    {
        public static T AddSetuppableComponent<T>(this GameObject gameObject) where T : Component, ILevelSetuppable
        {
            return LevelSetupper.Instance.AddSetuppableComponent<T>(gameObject);
        }
    }
}
