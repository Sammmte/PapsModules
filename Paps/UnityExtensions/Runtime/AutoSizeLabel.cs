using UnityEngine.UIElements;

namespace Paps.UnityExtensions
{
    [UxmlElement]
    public partial class AutoSizeLabel : Label
    {
        private const string USS_CLASS_NAME = "auto-size-label";

        [UxmlAttribute] public float MinFontSize { get; set; } = 5f;
        [UxmlAttribute] public float MaxFontSize { get; set; } = 100f;

        public AutoSizeLabel()
        {
            AddToClassList(USS_CLASS_NAME);
            RegisterCallback<GeometryChangedEvent>(AdjustFontSize);
            RegisterCallback<ChangeEvent<string>>(OnLabelChanged);
        }

        private void OnLabelChanged(ChangeEvent<string> ev)
        {
            AdjustFontSize();
        }

        private void AdjustFontSize(GeometryChangedEvent ev)
        {
            AdjustFontSize();
        }

        public void AdjustFontSize()
        {
            if (string.IsNullOrEmpty(text) || parent == null) return;

            float fontSize = MaxFontSize;

            var availableWidth = resolvedStyle.width;
            var availableHeight = resolvedStyle.height;

            style.fontSize = fontSize;

            while (fontSize > MinFontSize)
            {
                var measuredSize = MeasureTextSize(text, 0, MeasureMode.Undefined, 0, MeasureMode.Undefined);

                if (measuredSize.x <= availableWidth && measuredSize.y <= availableHeight)
                    break;

                fontSize -= 1f;
                style.fontSize = fontSize;
            }
        }
    }
}
