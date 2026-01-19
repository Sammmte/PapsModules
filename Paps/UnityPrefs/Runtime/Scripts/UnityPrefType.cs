namespace Paps.UnityPrefs
{
    public enum UnityPrefType
    {
        #if UNITY_EDITOR
        ProjectPrefs,
        UserProjectPrefs,
        #endif
        PlayerPrefsFileBased
    }
}
