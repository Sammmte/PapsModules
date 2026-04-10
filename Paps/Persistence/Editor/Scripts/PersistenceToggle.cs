using Paps.UnityToolbarExtenderUIToolkit;
using UnityEngine.UIElements;
using UnityMainToolbarElementAttribute = UnityEditor.Toolbars.MainToolbarElementAttribute;
using UnityMainToolbarElement = UnityEditor.Toolbars.MainToolbarElement;
using Paps.UnityPrefs;
using UnityEngine;

namespace Paps.Persistence.Editor
{
    [MainToolbarElement("PersistenceToggle")]
    public class PersistenceToggle : Toggle
    {
        private const string SAVE_ID = "persistence-toggle-value";
        private const string SCOPE = "persistence-editor";

        [UnityMainToolbarElementAttribute("PersistenceToggle")]
        private static UnityMainToolbarElement CreateDummy() => null;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void InitializePersistence()
        {
            if(UnityPrefs.UnityPrefs.GetPref(UnityPrefType.UserProjectPrefs, SCOPE).TryGet<bool>(SAVE_ID, out var value))
            {
                Persistence.PersistenceEnabled = value;
            }
        }

        public void InitializeElement()
        {
            label = "Persistence";

            value = Persistence.PersistenceEnabled;

            this.RegisterValueChangedCallback(ev =>
            {
                Persistence.PersistenceEnabled = ev.newValue;
                UnityPrefs.UnityPrefs.GetPref(UnityPrefType.UserProjectPrefs, SCOPE).Set(SAVE_ID, ev.newValue);
            });
        }
    }
}
