using Paps.SceneLoading;
using SaintsField;
using SaintsField.Playa;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Paps.LevelSetup
{
    [CreateAssetMenu(menuName = "Paps/Levels/Level", fileName = "Level Asset")]
    public class Level : ScriptableObject
    {
        public static Level Create(string id, Scene[] initialScenes)
        {
            var newLevel = CreateInstance<Level>();

            newLevel._id = id;
            newLevel._initialScenesGroup = initialScenes;

            return newLevel;
        }

        [SerializeField] private string _id;
        [SerializeField] private Scene[] _initialScenesGroup;

        public string Id => _id;
        public Scene[] InitialScenesGroup => _initialScenesGroup;
        [ShowInInspector] public Scene ActiveScene => InitialScenesGroup[0];


#if UNITY_EDITOR
        [field: SerializeField] public bool IsTestLevel { get; private set; }
        [field: SerializeField] public Scene[] ExtraScenes { get; private set; }

        public Scene[] GetRelatedScenes()
        {
            IEnumerable<Scene> scenes = new Scene[0];

            if(InitialScenesGroup != null)
                scenes = scenes.Concat(InitialScenesGroup);

            if(ExtraScenes != null)
                scenes = scenes.Concat(ExtraScenes);

            return scenes.ToArray();
        }
#endif
    }
}
