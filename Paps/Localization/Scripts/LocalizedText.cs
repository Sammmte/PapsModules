using System;

namespace Paps.Localization
{
    public abstract class LocalizedText
    {
        public abstract string Text { get; }
        public abstract event Action<string> OnTextChanged;

        public static implicit operator string(LocalizedText text) => text.Text;
    }
}
