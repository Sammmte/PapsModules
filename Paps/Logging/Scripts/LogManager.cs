using System;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Debug = UnityEngine.Debug;

namespace Paps.Logging
{
    public static class LogManager
    {
        private static LogConfiguration _configuration;
        private static GameLogHandler _handler = new GameLogHandler();

        private static LogConfiguration Configuration
        {
            get
            {
#if UNITY_EDITOR
                if (_configuration == null && !Application.isPlaying)
                    _configuration = LoadConfigurationOnEditor();
#endif
                return _configuration;

            }

            set => _configuration = value;
        }

        public static bool LogEnabled
        {
            get => Debug.unityLogger.logEnabled;
            set => Debug.unityLogger.logEnabled = value;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        public static void Initialize()
        {
            Debug.unityLogger.logHandler = _handler;
            Configuration = Addressables.LoadAssetAsync<LogConfiguration>("LogConfiguration").WaitForCompletion();
        }

        [Conditional("PAPS_LOG"), Conditional("UNITY_EDITOR")]
        public static void Log<T>(this T caller, string message)
        {
            if (!IsLogEnabled(typeof(T)))
                return;

            Debug.Log($"[{typeof(T).Name}] {message}");
        }

        private static bool IsLogEnabled(Type type)
        {
            if (!LogEnabled)
                return false;

            if (Configuration == null)
                return true;

            foreach(var logConfiguration in Configuration.LogConfigurationByTypes)
                if(logConfiguration.Type.Type == type)
                    return logConfiguration.Enabled;

            return Configuration.EnabledByDefault;
        }

#if UNITY_EDITOR
        public static LogConfiguration LoadConfigurationOnEditor()
        {
            var guid = UnityEditor.AssetDatabase.FindAssets($"t:{nameof(LogConfiguration)}").First();

            return UnityEditor.AssetDatabase.LoadAssetAtPath<LogConfiguration>(UnityEditor.AssetDatabase.GUIDToAssetPath(guid));
        }
#endif
    }
}
