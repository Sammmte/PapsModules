using UnityEngine;

namespace Paps.EditorGameSetup
{
    [CreateAssetMenu(menuName = "Paps/Editor Game Setup/Settings Asset")]
    public class EditorGameSetupSettings : ScriptableObject
    {
        [field: SerializeField] public EditorGameSetupper[] OrderedSetuppers { get; private set; }
    }
}
