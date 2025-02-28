using Paps.LevelSetup;
using Paps.SceneLoading;
using UnityEngine;

namespace Paps.DevelopmentTools.Editor
{
    [CreateAssetMenu(menuName = "Paps/Levels/Level Editor Data", fileName = "LevelEditorData")]
    public class LevelEditorData : ScriptableObject
    {
        [field: SerializeField] public ScriptableLevel Level { get; private set; }
        [SerializeField] private SerializableSceneGroup _extraScenes;

        public SceneGroup ExtraScenes => _extraScenes;
    }
}
