using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Paps.Cheats
{
    public static class CheatsInitializer
    {
#if UNITY_EDITOR || CHEATS
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        public static void Initialize()
        {
            Load().Forget();
        }
#endif

        private static async UniTaskVoid Load()
        {
            var cheatsPrefab = await CheatsHelper.LoadAssetAsync<GameObject>("CheatsPrefab");

            var instance = GameObject.Instantiate(cheatsPrefab);
            GameObject.DontDestroyOnLoad(instance);
            var cheatsUIDocument = instance.GetComponent<CheatsUIDocument>();
            await cheatsUIDocument.Initialize();
        }
    }
}
