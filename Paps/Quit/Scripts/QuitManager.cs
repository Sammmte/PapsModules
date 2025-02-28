using UnityEngine;

namespace Paps.Quit
{
    public static class QuitManager
    {
        private static QuitHandling? _quitHandling;

        public static IQuitHandler QuitHandler { get; set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            Application.wantsToQuit += HandleQuit;
        }

        private static bool HandleQuit()
        {
            if (_quitHandling == QuitHandling.Force || 
                (_quitHandling == null && QuitHandler == null) ||
                QuitHandler == null)
                return true;

            _quitHandling = null;

            return QuitHandler.OnWantsToQuit();
        }

        public static void Quit(QuitHandling quitHandling = QuitHandling.UseHandler)
        {
            _quitHandling = quitHandling;
#if UNITY_EDITOR
            if(HandleQuit())
                UnityEditor.EditorApplication.ExitPlaymode();
#else
            UnityEngine.Application.Quit();
#endif
        }
    }
}