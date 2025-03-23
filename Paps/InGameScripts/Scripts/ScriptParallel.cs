using Cysharp.Threading.Tasks;
using System;
using TNRD;
using UnityEngine;
using System.Linq;

namespace Paps.InGameScripts
{
    [Serializable]
    public class ScriptParallel : IScript
    {
        [SerializeField] private SerializableInterface<IScript>[] _scripts;

        private IScript[] _byConstructorScripts;
        
        public ScriptParallel() {}

        public ScriptParallel(IScript[] scripts)
        {
            _byConstructorScripts = scripts;
        }

        public async UniTask Execute()
        {
            if(_byConstructorScripts != null)
                await UniTask.WhenAll(_byConstructorScripts.Select(script => script.Execute()));
            else if (_scripts != null)
                await UniTask.WhenAll(_scripts.Select(script => script.Value.Execute()));
        }
    }
}
