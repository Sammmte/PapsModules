using Paps.LevelSetup;
using Paps.UnityToolbarExtenderUIToolkit;
using UnityEngine;
using UnityEngine.UIElements;

namespace Paps.DevelopmentTools.Editor
{
    [MainToolbarElement("CurrentLoadedLevelLabel", ToolbarAlign.Right, order: -999)]
    public class CurrentLoadedLevelLabel : Label
    {
        public void InitializeElement()
        {
            EditorLevelManager.OnLevelChanged += UpdateLabel;
            style.unityTextAlign = TextAnchor.MiddleCenter;
            UpdateLabel(EditorLevelManager.CurrentLoadedLevel);
        }

        private void UpdateLabel(Level? level)
        {
            if(level == null)
            {
                text = "Current level: None";
                style.color = new StyleColor(Color.red);
            }
            else
            {
                text = "Current level: " + level.Value.Name;
                style.color = new StyleColor(Color.green);
            }
        }
    }
}
