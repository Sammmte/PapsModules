using UnityEngine.UIElements;

namespace Paps.Build
{
    public interface IBuildWindowSettings
    {
        public string Title { get; }
        public VisualElement GetVisualElement();
        public object GetSettingsObject();
    }
}