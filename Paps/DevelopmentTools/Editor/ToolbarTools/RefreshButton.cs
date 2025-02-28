using Paps.UnityToolbarExtenderUIToolkit;
using UnityEngine.UIElements;
using UnityEditor.Compilation;
using UnityEditor;

namespace Paps.DevelopmentTools.Editor
{
    [MainToolbarElement("RefreshButton", ToolbarAlign.Right, order: -998)]
    public class RefreshButton : Button
    {
        public void InitializeElement()
        {
            text = "Refresh";
            clicked += CompilationPipeline.RequestScriptCompilation;
            clicked += AssetDatabase.Refresh;
        }
    }
}
