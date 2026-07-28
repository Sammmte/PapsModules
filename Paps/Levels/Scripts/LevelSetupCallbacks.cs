using Cysharp.Threading.Tasks;
using System;

namespace Paps.Levels
{
    public struct LevelSetupCallbacks
    {
        public Func<UniTask> AfterWillUnload;
        public Func<UniTask> AfterUnload;
    }
}
