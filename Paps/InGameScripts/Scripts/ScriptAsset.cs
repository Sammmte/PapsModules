using Cysharp.Threading.Tasks;
using TNRD;
using UnityEngine;

namespace Paps.InGameScripts
{
    [CreateAssetMenu(menuName = "Paps/Narrative Scripting/Script Asset")]
    public class ScriptAsset : ScriptableObject, IScript
    {
        [SerializeField] private SerializableInterface<IScript> _script;
        public UniTask Execute()
        {
            return _script.Value.Execute();
        }
    }
}
