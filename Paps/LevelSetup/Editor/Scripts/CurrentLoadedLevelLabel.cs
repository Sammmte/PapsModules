using Paps.LevelSetup;
using Paps.UnityToolbarExtenderUIToolkit;
using UnityEngine;
using UnityEngine.UIElements;
using UnityMainToolbarElementAttribute = UnityEditor.Toolbars.MainToolbarElementAttribute;
using UnityMainToolbarElement = UnityEditor.Toolbars.MainToolbarElement;

namespace Paps.LevelSetup.Editor
{
    [MainToolbarElement("CurrentLoadedLevelLabel")]
    public class CurrentLoadedLevelLabel : Label
    {
        [UnityMainToolbarElementAttribute("CurrentLoadedLevelLabel")]
        private static UnityMainToolbarElement CreateDummy() => null;
        
        public void InitializeElement()
        {
            EditorLevelManager.OnLevelChanged += UpdateLabel;
            style.unityTextAlign = TextAnchor.MiddleCenter;
            UpdateLabel(EditorLevelManager.CurrentLoadedLevel);
        }

        private void UpdateLabel(Level level)
        {
            if(level == null)
            {
                text = "Current level: None";
                style.color = new StyleColor(Color.red);
            }
            else
            {
                text = "Current level: " + level.Id;
                style.color = new StyleColor(Color.green);
            }
        }
    }
}
