using Paps.Levels;
using UnityEngine;

namespace Paps.GameplayTags
{
    [DisallowMultipleComponent]
    public abstract class GameplayTagsBase : MonoBehaviour, ILevelBound
    {
        public void Construct()
        {
            GameplayTagsManager.Instance.Register(this);
        }

        private void OnDestroy()
        {
            GameplayTagsManager.Instance.Unregister(this);
        }
    }
}
