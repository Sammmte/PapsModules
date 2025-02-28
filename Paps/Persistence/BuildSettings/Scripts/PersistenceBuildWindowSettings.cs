using UnityEngine;
using Paps.Build;
using UnityEngine.UIElements;

namespace Paps.Persistence.Build
{
    public class PersistenceBuildWindowSettings : IBuildWindowSettings
    {
        public string Title => "Persistence";

        private VisualElement _container;
        private Toggle _persistenceToggle;

        public PersistenceBuildWindowSettings()
        {
            _container = new VisualElement();
            _persistenceToggle = new Toggle("Enabled");

            _container.Add(_persistenceToggle);
        }

        public object GetSettingsObject()
        {
            return new PersistenceBuildSettings()
            {
                PersistenceEnabled = _persistenceToggle.value
            };
        }

        public VisualElement GetVisualElement()
        {
            return _container;
        }
    }
}
