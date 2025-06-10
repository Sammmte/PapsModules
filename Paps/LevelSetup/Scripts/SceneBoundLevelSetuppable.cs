using Paps.SceneLoading;

namespace Paps.LevelSetup
{
    public readonly struct SceneBoundLevelSetuppable
    {
        public ILevelSetuppable LevelSetuppable { get; }
        public Scene Scene { get; }

        public SceneBoundLevelSetuppable(ILevelSetuppable levelSetuppable, Scene scene)
        {
            LevelSetuppable = levelSetuppable;
            Scene = scene;
        }
    }
}