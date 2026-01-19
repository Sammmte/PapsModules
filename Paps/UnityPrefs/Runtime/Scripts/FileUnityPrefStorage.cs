using System.Collections.Generic;
using System.IO;

namespace Paps.UnityPrefs
{
    internal class FileUnityPrefStorage : IUnityPrefStorage
    {
        private string _basePath;
        private string _scope;
        private ISerializer _serializer;
        private string _filePath;
        private string _directoryPath;

        private Dictionary<string, SerializedValue> _serializedValues;

        public FileUnityPrefStorage(string basePath, string scope, ISerializer serializer)
        {
            _basePath = basePath;
            _scope = scope;
            _serializer = serializer;
            _filePath = GetFilePath();
            _directoryPath = Path.GetDirectoryName(_filePath);
        }

        public void Save<T>(string key, T value)
        {
            EnsureValuesAreLoaded();

            _serializedValues[key] = new SerializedValue()
            {
                SerializedString = _serializer.Serialize(value)
            };

            SaveAll();
        }

        public bool TryLoad<T>(string key, out T value)
        {
            EnsureValuesAreLoaded();

            if(_serializedValues.ContainsKey(key))
            {
                value = _serializer.Deserialize<T>(_serializedValues[key].SerializedString);
                return true;
            }

            value = default;
            return false;
        }

        private void EnsureValuesAreLoaded()
        {
            if(_serializedValues == null)
            {
                Load();
            }
        }

        private void Load()
        {
            EnsureDirectoryExists();

            if(!File.Exists(_filePath))
            {
                _serializedValues = new Dictionary<string, SerializedValue>();
                return;
            }

            _serializedValues = _serializer.Deserialize<Dictionary<string, SerializedValue>>(File.ReadAllText(_filePath));
        }

        private void SaveAll()
        {
            EnsureDirectoryExists();

            File.WriteAllText(GetFilePath(), _serializer.Serialize(_serializedValues));
        }

        private void EnsureDirectoryExists()
        {
            if(!Directory.Exists(_directoryPath))
                Directory.CreateDirectory(_directoryPath);
        }

        private string GetFilePath()
        {
            return Path.Combine(_basePath, $"{_scope}.prefs");
        }
    }
}
