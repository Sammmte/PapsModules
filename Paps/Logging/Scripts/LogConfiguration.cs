using UnityEngine;

namespace Paps.Logging
{
    [CreateAssetMenu(menuName = "Paps/Logging/Log Configuration")]
    public class LogConfiguration : ScriptableObject
    {
        [field: SerializeField] public bool EnabledByDefault { get; private set; }
        [field: SerializeField] public LogConfigurationByType[] LogConfigurationByTypes { get; private set; }
    }
}
