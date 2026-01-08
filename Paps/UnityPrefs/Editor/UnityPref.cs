namespace Paps.UnityPrefs
{
    public sealed class UnityPref
    {
        public UnityPrefType Type { get; }
        public string Scope { get; }
        private IUnityPrefStorage _storage;

        internal UnityPref(UnityPrefType type, string scope, IUnityPrefStorage storage)
        {
            Type = type;
            Scope = scope;
            _storage = storage;
        }

        public T Get<T>(string key, T defaultValue = default)
        {
            if(_storage.TryLoad<T>(key, out var value))
            {
                return value;
            }

            return defaultValue;
        }

        public bool TryGet<T>(string key, out T value) => _storage.TryLoad<T>(key, out value);

        public void Set<T>(string key, T value)
        {
            _storage.Save(key, value);
        }
    }
}
