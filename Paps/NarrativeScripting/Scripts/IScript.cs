using Cysharp.Threading.Tasks;

namespace Paps.NarrativeScripting
{
    public interface IScript
    {
        public UniTask Execute();
    }
}
