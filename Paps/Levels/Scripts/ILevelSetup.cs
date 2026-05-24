using Cysharp.Threading.Tasks;
using System.Threading;

namespace Paps.Levels
{
    public interface ILevelSetup
    {
        public void LevelLoaded() { }
        public UniTask Load(CancellationToken cancellationToken) => UniTask.CompletedTask;
        public UniTask Setup(CancellationToken cancellationToken) => UniTask.CompletedTask;
        public void Kickstart() { }
        public void Unload() { }
    }
}
