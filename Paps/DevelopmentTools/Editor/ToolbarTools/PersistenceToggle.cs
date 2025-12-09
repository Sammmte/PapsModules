using Paps.Persistence;
using Paps.UnityToolbarExtenderUIToolkit;
using UnityEngine.UIElements;
using UnityMainToolbarElementAttribute = UnityEditor.Toolbars.MainToolbarElementAttribute;
using UnityMainToolbarElement = UnityEditor.Toolbars.MainToolbarElement;

namespace Paps.DevelopmentTools.Editor
{
    [MainToolbarElement("PersistenceToggle")]
    public class PersistenceToggle : Toggle
    {
        [UnityMainToolbarElementAttribute("PersistenceToggle")]
        private static UnityMainToolbarElement CreateDummy() => null;
        
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
