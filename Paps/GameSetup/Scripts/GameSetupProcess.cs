using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Paps.GameSetup
{
    public abstract class GameSetupProcess : ScriptableObject
    {
        public abstract UniTask Setup();
    }
}
