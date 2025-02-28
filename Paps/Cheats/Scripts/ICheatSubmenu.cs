using Cysharp.Threading.Tasks;
using UnityEngine.UIElements;

namespace Paps.Cheats
{
    public interface ICheatSubmenu
    {
        public string DisplayName { get; }
        public int Order { get => 0; }
        public UniTask Load();
        public VisualElement GetVisualElement();
        public void OnShow() { }
        public void OnHide() { }
    }
}
