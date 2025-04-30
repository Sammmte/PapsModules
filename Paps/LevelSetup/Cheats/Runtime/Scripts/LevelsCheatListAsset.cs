using UnityEngine;

namespace Paps.LevelSetup.Cheats
{
    [CreateAssetMenu(menuName = "Paps/Levels/Levels Cheat List", fileName = "LevelsCheatList")]
    public class LevelsCheatListAsset : ScriptableObject
    {
        [SerializeField] public ScriptableLevel[] Levels;
    }
}