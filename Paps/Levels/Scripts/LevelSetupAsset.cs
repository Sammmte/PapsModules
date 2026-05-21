using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace Paps.Levels
{
    public abstract class LevelSetupAsset : ScriptableObject, ILevelSetup
    {
        public virtual UniTask Load(CancellationToken cancellationToken) => UniTask.CompletedTask;
        public virtual UniTask Setup(CancellationToken cancellationToken) => UniTask.CompletedTask;
        public virtual void Kickstart() { }
        public virtual void Unload() { }
    }
}
