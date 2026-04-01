using Cysharp.Threading.Tasks;

namespace Paps.Levels
{
    public interface ILevelSetup
    {
        public UniTask Loaded() => UniTask.CompletedTask;
        public UniTask Setup() => UniTask.CompletedTask;
        public UniTask Kickstart() => UniTask.CompletedTask;
        public UniTask Unload() => UniTask.CompletedTask;
    }
}
