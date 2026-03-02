using Cysharp.Threading.Tasks;

namespace Paps.Levels
{
    public interface ILevelSetuppable
    {
        public void Created()
        {
            
        }
        public UniTask Setup()
        {
            return UniTask.CompletedTask;
        }

        public void Kickstart()
        {

        }

        public UniTask Unload()
        {
            return UniTask.CompletedTask;
        }
    }
}
