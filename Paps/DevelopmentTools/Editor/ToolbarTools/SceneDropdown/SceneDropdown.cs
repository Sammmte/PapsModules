using Paps.UnityToolbarExtenderUIToolkit;
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;

namespace Paps.DevelopmentTools.Editor
{
    [MainToolbarElement("SceneDropdown", ToolbarAlign.Right, order: -1000)]
    public class SceneDropdown : EditorToolbarDropdown
    {
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
