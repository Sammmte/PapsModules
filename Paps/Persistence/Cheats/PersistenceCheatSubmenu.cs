#if PERSISTENCE || UNITY_EDITOR
using Cysharp.Threading.Tasks;
using Paps.Cheats;
using UnityEngine.UIElements;

namespace Paps.Persistence.Cheats
{

    public class PersistenceCheatSubmenu : ICheatSubmenu
    {
        public string DisplayName => "Persistence";

        private VisualElement _container;
        private Toggle _persistenceEnabledToggle;

        public PersistenceCheatSubmenu()
        {
            _container = new VisualElement();

            _persistenceEnabledToggle = new Toggle("Enabled");
            _persistenceEnabledToggle.value = StorageHandler.PersistenceEnabled;

            _persistenceEnabledToggle.RegisterValueChangedCallback(ev => StorageHandler.PersistenceEnabled = ev.newValue);

            _container.Add(_persistenceEnabledToggle);
        }

        public VisualElement GetVisualElement()
        {
            return _container;
        }

        public async UniTask Load()
        {
            
        }
    }
}
#endif