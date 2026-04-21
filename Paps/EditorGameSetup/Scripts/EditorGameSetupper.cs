using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Paps.EditorGameSetup
{
    public abstract class EditorGameSetupper : ScriptableObject
    {
        public abstract UniTask Setup(EditorGameSetupContext context);
    }
}
