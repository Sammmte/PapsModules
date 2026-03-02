using UnityEngine;

namespace Paps.Levels.Cheats
{
    [CreateAssetMenu(menuName = "Paps/Levels/Levels Cheat List", fileName = "LevelsCheatList")]
    public class LevelsCheatListAsset : ScriptableObject
    {
        [SerializeField] public Level[] Levels;
    }
}