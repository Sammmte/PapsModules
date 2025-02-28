using Paps.Persistence;
using Paps.UnityToolbarExtenderUIToolkit;
using UnityEngine;
using UnityEngine.UIElements;

namespace Paps.DevelopmentTools.Editor
{
    [MainToolbarElement("PersistenceToggle")]
    public class PersistenceToggle : Toggle
    {
        [Serialize] private bool _enabled;

        public void InitializeElement()
        {
            label = "Persistence";

            value = _enabled;
            SetPersistence();

            this.RegisterValueChangedCallback(ev =>
            {
                _enabled = ev.newValue;
                SetPersistence();
            });
        }

        private void SetPersistence()
        {
            StorageHandler.PersistenceEnabled = _enabled;
        }
    }
}
