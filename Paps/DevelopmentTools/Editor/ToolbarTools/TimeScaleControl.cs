using Paps.UnityToolbarExtenderUIToolkit;
using UnityEngine.UIElements;
using UnityMainToolbarElementAttribute = UnityEditor.Toolbars.MainToolbarElementAttribute;
using UnityMainToolbarElement = UnityEditor.Toolbars.MainToolbarElement;
using UnityEngine;

namespace Paps.DevelopmentTools.Editor
{
    [MainToolbarElement("TimeScaleControl")]
    public class TimeScaleControl : FloatField
    {
        [UnityMainToolbarElementAttribute("TimeScaleControl")]
        private static UnityMainToolbarElement CreateDummy() => null;

        [Serialize] private float _scale;
        
        public void InitializeElement()
        {
            label = "Time Scale";
            isDelayed = true;

            SetValueWithoutNotify(_scale);

            this.RegisterValueChangedCallback(ev => Apply());
            
            Apply();
        }

        private void Apply()
        {
            _scale = value;

            if(!Application.isPlaying)
                return;

            Time.timeScale = value;
            
            _scale = Time.timeScale;
            SetValueWithoutNotify(Time.timeScale);
        }
    }
}
