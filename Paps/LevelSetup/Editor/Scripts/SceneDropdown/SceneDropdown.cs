using Paps.UnityToolbarExtenderUIToolkit;
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;
using MainToolbarElement = Paps.UnityToolbarExtenderUIToolkit.MainToolbarElementAttribute;
using UnityMainToolbarElementAttribute = UnityEditor.Toolbars.MainToolbarElementAttribute;
using UnityMainToolbarElement = UnityEditor.Toolbars.MainToolbarElement;

namespace Paps.LevelSetup.Editor
{
    [MainToolbarElement("SceneDropdown")]
    public class SceneDropdown : EditorToolbarDropdown
    {
        [UnityMainToolbarElementAttribute("SceneDropdown")]
        private static UnityMainToolbarElement CreateDummy() => null;
        
        public void InitializeElement()
        {
            text = "Scenes";
            clicked += ShowDropdownWindow;
        }

        private void ShowDropdownWindow()
        {
            var window = EditorWindow.CreateInstance<SceneDropdownWindow>();

            window.ShowAsDropdownForMainToolbar(worldBound, new Vector2(400, 150));
        }
    }
}
