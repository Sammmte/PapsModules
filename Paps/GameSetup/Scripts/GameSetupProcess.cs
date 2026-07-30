using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace Paps.GameSetup
{
    public abstract class GameSetupProcess : ScriptableObject
    {
        public abstract UniTask Setup(CancellationToken cancellationToken);
    }
}
