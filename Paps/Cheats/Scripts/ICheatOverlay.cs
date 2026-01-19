using Cysharp.Threading.Tasks;
using UnityEngine.UIElements;

namespace Paps.Cheats
{
    public interface ICheatOverlay
    {
        public string Id { get; }
        public string DisplayName { get; }
        public UniTask Load();
        public VisualElement GetVisualElement();
        public void OnShow() { }
        public void OnHide() { }
    }
}
