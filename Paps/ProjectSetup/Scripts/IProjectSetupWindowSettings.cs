using UnityEngine.UIElements;

namespace Paps.ProjectSetup
{
    public interface IProjectSetupWindowSettings
    {
        public string Title { get; }
        public VisualElement GetVisualElement();
        public object GetSettingsObject();
    }
}