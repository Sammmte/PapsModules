using Cysharp.Threading.Tasks;
using System;
using TNRD;
using UnityEngine;
using System.Linq;

namespace Paps.NarrativeScripting
{
    [Serializable]
    public struct ScriptParallel : IScript
    {
        [SerializeField] private SerializableInterface<IScript>[] _scripts;

        private IScript[] _cachedScripts;

        private IScript[] Scripts
        {
            get
            {
                if (_cachedScripts == null)
                    _cachedScripts = _scripts.Select(script => script.Value).ToArray();

                return _cachedScripts;
            }
        }

        public ScriptParallel(IScript[] scripts)
        {
            _scripts = null;
            _cachedScripts = scripts;
        }

        public async UniTask Execute()
        {
            await UniTask.WhenAll(Scripts.Select(script => script.Execute()));
        }
    }
}
