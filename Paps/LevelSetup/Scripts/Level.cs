using Paps.SceneLoading;

namespace Paps.LevelSetup
{
    public readonly struct Level
    {
        public string Name { get; }
        public SceneGroup InitialScenesGroup { get; }
        public Scene ActiveScene { get; }

        public Level(string name, SceneGroup initialScenesGroup, Scene activeScene)
        {
            Name = name;
            InitialScenesGroup = initialScenesGroup;
            ActiveScene = activeScene;
        }
    }
}
