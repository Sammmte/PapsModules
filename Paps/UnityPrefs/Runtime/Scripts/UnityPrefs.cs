using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Paps.UnityPrefs
{
    public static class UnityPrefs
    {
        #if UNITY_EDITOR
        public static readonly string PROJECT_PREFS_BASE_FILE_PATH = Path.Combine(Application.dataPath, "~ProjectPrefs");
        public static readonly string USER_PROJECT_PREFS_BASE_FILE_PATH = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "UserSettings", "UserProjectPrefs");
        #endif
        public static readonly string PLAYER_PREFS_FILE_BASED_BASE_FILE_PATH = Path.Combine(Application.persistentDataPath, "PlayerPrefsFileBased");
        private static readonly ISerializer JSON_SERIALIZER = new JsonSerializer();

        private static Dictionary<UnityPrefType, Dictionary<string, UnityPref>> _prefs = new ()
        {
            #if UNITY_EDITOR
            { UnityPrefType.ProjectPrefs, new Dictionary<string, UnityPref>() },
            { UnityPrefType.UserProjectPrefs, new Dictionary<string, UnityPref>() },
            #endif
            { UnityPrefType.PlayerPrefsFileBased, new Dictionary<string, UnityPref>() }
        };

        public static UnityPref GetPref(UnityPrefType type, string scope)
        {
            var dictionary = _prefs[type];

            if(!dictionary.ContainsKey(scope))
            {
                dictionary[scope] = new UnityPref(type, scope, GetStorage(type, scope));
            }

            return dictionary[scope];
        }

        private static IUnityPrefStorage GetStorage(UnityPrefType type, string scope)
        {
            switch(type)
            {
                #if UNITY_EDITOR
                case UnityPrefType.ProjectPrefs:
                    return new FileUnityPrefStorage(PROJECT_PREFS_BASE_FILE_PATH, scope, JSON_SERIALIZER);
                case UnityPrefType.UserProjectPrefs:
                    return new FileUnityPrefStorage(USER_PROJECT_PREFS_BASE_FILE_PATH, scope, JSON_SERIALIZER);
                #endif
                case UnityPrefType.PlayerPrefsFileBased:
                    return new FileUnityPrefStorage(PLAYER_PREFS_FILE_BASED_BASE_FILE_PATH, scope, JSON_SERIALIZER);
                default:
                    return null;
            }
        }
    }
}
