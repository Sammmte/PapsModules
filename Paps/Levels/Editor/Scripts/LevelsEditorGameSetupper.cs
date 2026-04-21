using Cysharp.Threading.Tasks;
using Paps.EditorGameSetup;

namespace Paps.Levels.Editor
{
    public class LevelsEditorGameSetupper : EditorGameSetupper
    {
        public override async UniTask Setup(EditorGameSetupContext context)
        {
            var scenes = context.LoadedScenesWhenPlayModeStarted;

            var bestMatchLevel = EditorLevelManager.GetBestLevelMatchForScenes(scenes);

            Level finalLevel = null;

            if(bestMatchLevel != null)
            {
                finalLevel = Level.Create($"EditorSetupLevel_{bestMatchLevel.Id}", scenes, bestMatchLevel.LevelSetups);
            }
            else
            {
                finalLevel = Level.Create("EditorSetupLevel", scenes);
            }

            await LevelManager.Instance.LoadLevel(finalLevel);
        }
    }
}