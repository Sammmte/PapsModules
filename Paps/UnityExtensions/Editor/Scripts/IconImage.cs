using UnityEditor;
using UnityEngine.UIElements;

namespace Paps.UnityExtensions.Editor
{
    [UxmlElement]
    public partial class IconImage : Image
    {
        private string _icon;

        [UxmlAttribute] public string Icon
        {
            get => _icon;
            set
            {
                _icon = value;

                try
                {
                    var content = EditorGUIUtility.IconContent(_icon);
                    image = content.image;
                }
                catch
                {
                    image = null;
                }
            }
        }
    }
}
