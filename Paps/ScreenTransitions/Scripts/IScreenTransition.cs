using Cysharp.Threading.Tasks;

namespace Paps.ScreenTransitions
{
    public interface IScreenTransition
    {
        public UniTask PlayIn();
        public UniTask PlayOut();
    }
}
