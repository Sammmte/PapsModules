using Unity.Profiling;

namespace Paps.Localization
{
    internal static class LocalizationProfiling
    {
        private const string BASE_MARKER_NAME = "Paps.Localization";

        public static readonly ProfilerMarker GET_LOCALIZED_STRING_MARKER = 
            new ProfilerMarker($"{BASE_MARKER_NAME}.GetLocalizedString");
        public static readonly ProfilerMarker<int> GET_LOCALIZED_STRING_WITH_PARAMETERS_MARKER = 
            new ProfilerMarker<int>($"{BASE_MARKER_NAME}.GetLocalizedStringWithParameters", "Total Parameters");
    }
}