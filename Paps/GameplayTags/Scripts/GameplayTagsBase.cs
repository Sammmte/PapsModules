using Cysharp.Threading.Tasks;
using Paps.LevelSetup;
using System.Collections.Generic;
using UnityEngine;

namespace Paps.GameplayTags
{
    [DisallowMultipleComponent]
    public abstract class GameplayTagsBase : MonoBehaviour, ILevelSetuppable
    {
        private void Awake()
        {
            GameplayTagsManager.Instance.Register(this);
        }

        public async UniTask Setup()
        {
            GameplayTagsManager.Instance.Register(this);
        }

        public async UniTask Unload()
        {
            GameplayTagsManager.Instance.Unregister(this);
        }

        private void OnDestroy()
        {
            GameplayTagsManager.Instance.Unregister(this);
        }
    }
}
