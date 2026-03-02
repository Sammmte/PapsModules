using Paps.SceneLoading;

namespace Paps.Levels
{
    public readonly struct SceneBoundLevelSetuppable
    {
        public ILevelBound LevelSetuppable { get; }
        public Scene Scene { get; }

        public SceneBoundLevelSetuppable(ILevelBound levelSetuppable, Scene scene)
        {
            LevelSetuppable = levelSetuppable;
            Scene = scene;
        }
    }
}