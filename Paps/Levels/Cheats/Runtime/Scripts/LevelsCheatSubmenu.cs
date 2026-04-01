using Cysharp.Threading.Tasks;
using Paps.Cheats;
using UnityEngine.UIElements;

namespace Paps.Levels.Cheats
{
    public class LevelsCheatSubmenu : ICheatSubmenu
    {
        private VisualElement _container;
        public string DisplayName => "Levels";
        
        public async UniTask Load()
        {
            _container = new VisualElement();
        }

        public VisualElement GetVisualElement()
        {
            return _container;
        }

        public void OnShow()
        {
            PrepareLevelButtons();
        }

        private void PrepareLevelButtons()
        {
            _container.Clear();

            if (LevelManager.Instance.LevelList.Count == 0)
            {
                _container.Add(new Label("No levels found!"));
                return;
            }

            for (int i = 0; i < LevelManager.Instance.LevelList.Count; i++)
            {
                _container.Add(CreateButtonForLevel(LevelManager.Instance.LevelList[i]));
            }
        }

        private Button CreateButtonForLevel(Level level)
        {
            var button = new Button();
            button.text = level.Id;
            button.clicked += () => LevelManager.Instance.LoadLevel(level);

            return button;
        }
    }
}
