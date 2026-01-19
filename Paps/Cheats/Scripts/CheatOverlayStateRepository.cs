using Paps.UnityPrefs;
using System.Collections.Generic;

namespace Paps.Cheats
{
    internal class CheatOverlayStateRepository
    {
        private const string PREF_SCOPE = "cheat-overlays";
        private const string CHEAT_OVERLAY_STATES_SAVE_KEY = "cheat-overlay-states";

        private UnityPref _pref;

        public CheatOverlayStateRepository()
        {
            _pref = UnityPrefs.UnityPrefs.GetPref(UnityPrefType.PlayerPrefsFileBased, PREF_SCOPE);
        }

        public Dictionary<string, CheatOverlayState> Get() => _pref.Get(CHEAT_OVERLAY_STATES_SAVE_KEY, 
            new Dictionary<string, CheatOverlayState>());

        public void Save(Dictionary<string, CheatOverlayState> data) => _pref.Set(CHEAT_OVERLAY_STATES_SAVE_KEY, data);
    }
}
