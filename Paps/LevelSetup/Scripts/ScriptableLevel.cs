using Eflatun.SceneReference;
using Paps.SceneLoading;
using UnityEngine;

namespace Paps.LevelSetup
{
    [CreateAssetMenu(menuName = "Paps/Levels/Level", fileName = "LevelAsset")]
    public class ScriptableLevel : ScriptableObject
    {
        [SerializeField] private string _name;
        [SerializeField] private SerializableSceneGroup _initialScenesGroup;
        [SerializeField] private SceneReference _activeScene;

        public string Name => _name;
        public SceneGroup InitialScenesGroup => _initialScenesGroup;
        public Scene ActiveScene => _activeScene;

        public static implicit operator Level(ScriptableLevel scriptableLevel)
        {
            return new Level(scriptableLevel.Name, scriptableLevel.InitialScenesGroup, scriptableLevel.ActiveScene);
        }
    }
}
