using Paps.SceneLoading;
using UnityScene = UnityEngine.SceneManagement.Scene;
using Scene = Paps.SceneLoading.Scene;

namespace Paps.DevelopmentTools.Editor
{
    public struct SceneDTO
    {
        public string Name;
        public string Path;

        public static implicit operator SceneDTO(Scene scene)
        {
            return new SceneDTO()
            {
                Name = scene.Name,
                Path = scene.Path
            };
        }

        public static implicit operator Scene(SceneDTO sceneDTO)
        {
            return new Scene(sceneDTO.Name, sceneDTO.Path);
        }

        public static implicit operator SceneDTO(UnityScene unityScene)
        {
            return new SceneDTO()
            {
                Name = unityScene.name,
                Path = unityScene.path
            };
        }
    }
}
