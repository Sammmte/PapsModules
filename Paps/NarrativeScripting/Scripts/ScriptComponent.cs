using Cysharp.Threading.Tasks;
using TNRD;
using UnityEngine;

namespace Paps.NarrativeScripting
{
    public class ScriptComponent : MonoBehaviour, IScript
    {
        [SerializeField] private SerializableInterface<IScript> _script;

        public UniTask Execute()
        {
            return _script.Value.Execute();
        }
    }
}
