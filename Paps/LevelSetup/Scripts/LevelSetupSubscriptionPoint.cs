using Cysharp.Threading.Tasks;
using TNRD;
using UnityEngine;
using System.Linq;

namespace Paps.LevelSetup
{
    public class LevelSetupSubscriptionPoint : MonoBehaviour
    {
        [SerializeField] private SerializableInterface<ILevelSetuppable>[] _levelSetuppables;

        private ILevelSetuppable[] _cached;

        private void Awake()
        {
            _cached = _levelSetuppables.Select(l => l.Value).ToArray();
            LevelSetupper.RegisterSceneBound(gameObject.scene, _cached);
        }
    }
}
