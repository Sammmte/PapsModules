using Cysharp.Threading.Tasks;
using System;
using TNRD;
using UnityEngine;
using System.Linq;

namespace Paps.NarrativeScripting
{
    [Serializable]
    public struct ScriptSequence : IScript
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

        public ScriptSequence(IScript[] scripts)
        {
            _scripts = null;
            _cachedScripts = scripts;
        }

        public async UniTask Execute()
        {
            var scripts = Scripts;

            for (int i = 0; i < scripts.Length; i++)
                await scripts[i].Execute();
        }
    }
}
