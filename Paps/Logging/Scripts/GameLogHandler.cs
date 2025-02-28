using System;
using UnityEngine;

namespace Paps.Logging
{
    public class GameLogHandler : ILogHandler
    {
        private ILogHandler _defaultHandler = Debug.unityLogger.logHandler;

        public void LogException(Exception exception, UnityEngine.Object context)
        {
#if PAPS_LOG || UNITY_EDITOR
            _defaultHandler.LogException(exception, context);
#endif
        }

        public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
        {
#if PAPS_LOG || UNITY_EDITOR
            _defaultHandler.LogFormat(logType, context, format, args);
#endif
        }
    }
}
