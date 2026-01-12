using Cysharp.Threading.Tasks;
using Paps.Cheats;
using UnityEngine.UIElements;

namespace Paps.LevelSetup.Cheats
{
    public class LevelsCheatSubmenu : ICheatSubmenu
    {
        private VisualElement _container;
        public string DisplayName => "Levels";
        
        public async UniTask Load()
        {
            var cheatLevels = await this.LoadAssetAsync<LevelsCheatListAsset>("LevelsCheatList");

            _container = new VisualElement();

            if (cheatLevels == null || cheatLevels.Levels.Length == 0)
            {
                _container.Add(new Label("No levels found!"));
                return;
            }

            for (int i = 0; i < cheatLevels.Levels.Length; i++)
            {
                _container.Add(CreateButtonForLevel(cheatLevels.Levels[i]));
            }
        }

        public VisualElement GetVisualElement()
        {
            return _container;
        }

        private Button CreateButtonForLevel(Level level)
        {
            var button = new Button();
            button.text = level.Id;
            button.clicked += () => LevelSetupper.Instance.LoadAndSetupLevel(level);

            return button;
        }
    }
}
