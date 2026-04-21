using Paps.SceneLoading;

namespace Paps.EditorGameSetup
{
    public readonly struct EditorGameSetupContext
    {
        public Scene[] LoadedScenesWhenPlayModeStarted { get; }

        public EditorGameSetupContext(Scene[] loadedScenesWhenPlayModeStarted)
        {
            LoadedScenesWhenPlayModeStarted = loadedScenesWhenPlayModeStarted;
        }
    }
}
