using Cysharp.Threading.Tasks;
using Paps.LevelSetup;
using System.Collections.Generic;
using UnityEngine;

namespace Paps.GameplayTags
{
    public abstract class GameplayTagsBase : MonoBehaviour, ILevelSetuppable
    {
        [field: SerializeField] public bool IncludeChildren { get; set; }
        [field: SerializeField] public List<GameObject> ExtraTaggedObjects { get; private set; }
        
        public async UniTask Setup()
        {
            GameplayTagsManager.Register(this);
        }

        public async UniTask Unload()
        {
            GameplayTagsManager.Unregister(this);
        }

        private void OnDestroy()
        {
            GameplayTagsManager.Unregister(this);
        }

        public bool IsGameObjectTagged(GameObject go)
        {
            if (gameObject == go)
                return true;
            
            if (IncludeChildren)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    if (transform.GetChild(i).gameObject == go)
                        return true;
                }
            }

            for (int i = 0; i < ExtraTaggedObjects.Count; i++)
            {
                if (ExtraTaggedObjects[i] == go)
                    return true;
            }

            return false;
        }
    }
}
