using Cysharp.Threading.Tasks;
using Paps.LevelSetup;
using System.Collections.Generic;
using UnityEngine;

namespace Paps.ComponentOwnership
{
    public class ComponentOwner : MonoBehaviour, ILevelSetuppable
    {
        [field: SerializeField] internal Component IdentityComponent { get; private set; }
        [field: SerializeField] internal List<Component> OwnedComponents { get; private set; }

        public void Created()
        {
            ComponentOwnershipManager.Instance.Register(this);
        }

        public async UniTask Unload()
        {
            ComponentOwnershipManager.Instance.Unregister(this);
        }
    }
}
