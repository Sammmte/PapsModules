using Cysharp.Threading.Tasks;
using System;
using TNRD;
using UnityEngine;
using System.Linq;

namespace Paps.NarrativeScripting
{
    [Serializable]
    public class ScriptSequence : IScript
    {
        [SerializeField] private SerializableInterface<IScript>[] _scripts;

        private IScript[] _byConstructorScripts;
        
        public ScriptSequence() {}

        public ScriptSequence(IScript[] scripts)
        {
            _byConstructorScripts = scripts;
        }

        public async UniTask Execute()
        {
            if(_byConstructorScripts != null)
                for (int i = 0; i < _byConstructorScripts.Length; i++)
                    await _byConstructorScripts[i].Execute();
            else if (_scripts != null)
                for (int i = 0; i < _scripts.Length; i++)
                    await _scripts[i].Value.Execute();
        }
    }
}
