using Paps.UnityToolbarExtenderUIToolkit;
using UnityEngine.UIElements;
using UnityEditor.Compilation;
using UnityEditor;
using UnityMainToolbarElementAttribute = UnityEditor.Toolbars.MainToolbarElementAttribute;
using UnityMainToolbarElement = UnityEditor.Toolbars.MainToolbarElement;

namespace Paps.DevelopmentTools.Editor
{
    [MainToolbarElement("RefreshButton")]
    public class RefreshButton : Button
    {
        [UnityMainToolbarElementAttribute("RefreshButton")]
        private static UnityMainToolbarElement CreateDummy() => null;
        
        public void InitializeElement()
        {
            text = "Refresh";
            clicked += CompilationPipeline.RequestScriptCompilation;
            clicked += AssetDatabase.Refresh;
        }
    }
}
