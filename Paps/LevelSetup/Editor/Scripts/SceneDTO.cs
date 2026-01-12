using UnityScene = UnityEngine.SceneManagement.Scene;
using Scene = Paps.SceneLoading.Scene;

namespace Paps.LevelSetup.Editor
{
    public struct SceneDTO
    {
        public string Path;
        public int BuildIndex;

        public static implicit operator SceneDTO(Scene scene)
        {
            return new SceneDTO()
            {
                Path = scene.Path,
                BuildIndex = scene.BuildIndex
            };
        }

        public static implicit operator Scene(SceneDTO sceneDTO)
        {
            return new Scene(sceneDTO.Path, sceneDTO.BuildIndex);
        }

        public static implicit operator SceneDTO(UnityScene unityScene)
        {
            return new SceneDTO()
            {
                Path = unityScene.path,
                BuildIndex = unityScene.buildIndex
            };
        }
    }
}
