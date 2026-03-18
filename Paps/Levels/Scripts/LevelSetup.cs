using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Paps.Levels
{
    public abstract class LevelSetup : ScriptableObject
    {
        public virtual UniTask Loaded() => UniTask.CompletedTask;
        public virtual UniTask Kickstart() => UniTask.CompletedTask;
        public virtual UniTask Unload() => UniTask.CompletedTask;
    }
}
