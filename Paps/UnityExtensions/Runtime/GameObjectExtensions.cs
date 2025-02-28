using UnityEngine;

namespace Paps.UnityExtensions
{
    public static class GameObjectExtensions
    {
        public static void DontDestroyOnLoad(this GameObject obj)
        {
            obj.transform.SetParent(null);
            GameObject.DontDestroyOnLoad(obj);
        }
    }
}
