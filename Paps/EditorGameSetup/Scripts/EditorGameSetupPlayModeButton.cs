using Paps.UnityToolbarExtenderUIToolkit;
using UnityMainToolbarElementAttribute = UnityEditor.Toolbars.MainToolbarElementAttribute;
using UnityMainToolbarElement = UnityEditor.Toolbars.MainToolbarElement;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;
using System.Runtime.Remoting.Contexts;
using System;

namespace Paps.EditorGameSetup
{
    [MainToolbarElement("EditorGameSetupPlayModeButton")]
    public class EditorGameSetupPlayModeButton : Button
    {
        [UnityMainToolbarElementAttribute("EditorGameSetupPlayModeButton")]
        private static UnityMainToolbarElement CreateDummy() => null;
        
        [Serialize] private EditorGameSetupMode _setupMode = EditorGameSetupMode.Custom;

        private EditorGameSetupMode[] _allModes;

        public void InitializeElement()
        {
            _allModes = Enum.GetValues(typeof(EditorGameSetupMode)) as EditorGameSetupMode[];

            iconImage = Background.FromTexture2D(EditorGUIUtility.IconContent("PlayButton").image as Texture2D);
            text = GetTextByMode();
            clicked += PlayModeButtonClicked;

            this.AddManipulator(new ContextualMenuManipulator(OnItemOptions));
        }

        private void OnItemOptions(ContextualMenuPopulateEvent ev)
        {
            foreach (var mode in _allModes)
            {
                ev.menu.AppendAction(mode.ToString(), a =>
                {
                    _setupMode = mode;
                    text = GetTextByMode();
                }, mode == _setupMode ? DropdownMenuAction.Status.Checked : DropdownMenuAction.Status.Normal);
            }
        }

        private string GetTextByMode()
        {
            switch (_setupMode)
            {
                case EditorGameSetupMode.Entry:
                    return "Entry";
                case EditorGameSetupMode.Custom:
                    return "Custom";
                case EditorGameSetupMode.NoSetup:
                    return "No Setup";
                default: 
                    return "Undefined";
            }
        }

        private void PlayModeButtonClicked()
        {
            EditorGameSetupManager.EnterPlayMode(new EditorGameSetupParameters()
            {
                SetupMode = _setupMode
            });
        }
    }
}
