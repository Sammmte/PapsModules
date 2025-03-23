using Cysharp.Threading.Tasks;

namespace Paps.InGameScripts
{
    public interface IScript
    {
        public UniTask Execute();
    }
}
