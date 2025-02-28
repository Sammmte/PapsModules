using Paps.Broadcasting;
using Paps.GlobalProvisioning;

namespace Paps.Pause
{
    public static class PauseManager
    {
        public static bool IsPaused { get; private set; }

        public static void Pause()
        {
            if (IsPaused)
                return;

            IsPaused = true;
            BroadcastChannel.Global.Raise(new PauseEvent());
        }

        public static void Unpause()
        {
            if (!IsPaused)
                return;

            IsPaused = false;
            BroadcastChannel.Global.Raise(new UnpauseEvent());
        }
    }
}
