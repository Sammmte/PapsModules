namespace Paps.UnityPrefs
{
    internal interface IUnityPrefStorage
    {
        public bool TryLoad<T>(string key, out T value);
        public void Save<T>(string key, T value);
    }
}
