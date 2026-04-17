using Cysharp.Threading.Tasks;
using Paps.GameSetup;
using UnityEngine;

namespace Paps.Cheats
{
    public class CheatGameSetupProcess : GameSetupProcess
    {
        public override async UniTask Setup()
        {
            var cheatsPrefab = await CheatsHelper.LoadAssetAsync<GameObject>("CheatsPrefab");

            var instance = GameObject.Instantiate(cheatsPrefab);
            GameObject.DontDestroyOnLoad(instance);
            var cheatsUIDocument = instance.GetComponent<CheatsUIDocument>();
            await cheatsUIDocument.Initialize();
        }
    }
}
