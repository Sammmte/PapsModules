using Paps.UnityToolbarExtenderUIToolkit;
using UnityMainToolbarElementAttribute = UnityEditor.Toolbars.MainToolbarElementAttribute;
using UnityMainToolbarElement = UnityEditor.Toolbars.MainToolbarElement;
using UnityEngine.UIElements;
using UnityEngine;

namespace Paps.EditorGameSetup
{
    [MainToolbarElement("EditorGameSetupModeDropdown")]
    public class EditorGameSetupModeDropdown : EnumField
    {
        [UnityMainToolbarElementAttribute("EditorGameSetupModeDropdown")]
        private static UnityMainToolbarElement CreateDummy() => null;
        
        [Serialize] private EditorGameSetupMode _setupMode = EditorGameSetupMode.Custom;

        public void InitializeElement()
        {
            Init(EditorGameSetupMode.Custom);

            label = "Setup Mode";
            value = _setupMode;

            this.RegisterValueChangedCallback(ev =>
            {
                _setupMode = (EditorGameSetupMode)ev.newValue;
                UpdateState();
            });

            if(!Application.isPlaying)
            {
                UpdateState();
            }
        }

        private void UpdateState()
        {
            EditorGameSetupManager.SaveState(new EditorGameSetupParameters()
            {
                SetupMode = _setupMode
            });
        }
    }
}
