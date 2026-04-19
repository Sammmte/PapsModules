using UnityEngine.Localization.Settings;
using Paps.UnityExtensions;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine.Localization.Tables;
using System.Linq;
using UnityEngine.Pool;

namespace Paps.Localization
{
    [DefaultExecutionOrder(-10000)]
    public class LocalizationManager : MonoBehaviour
    {
        private const string NO_LOCALIZED_KEY = "KEY {0} NOT FOUND IN TABLE {1}";
        private const string NO_LOCALIZED_TABLE = "REQUESTED KEY {0} BUT TABLE {1} IS NOT LOADED";
        private const string EMPTY_TABLE = "REQUESTED KEY {0} ON EMPTY TABLE";
        private const string EMPTY_KEY = "REQUESTED EMPTY KEY ON TABLE {0}";
        private const string EMPTY_KEY_AND_TABLE = "REQUESTED EMPTY KEY ON EMPTY TABLE";

        public static LocalizationManager Instance { get; private set; }

        [SerializeField] private int _cachedTextPerTableCapacity = 1000;
        [SerializeField] private int _maxTableCount = 10;

        private Language[] _availableLanguages;

        public event Action<Language> OnLanguageChanged;

        private Dictionary<string, StringTable> _loadedTables;
        private Dictionary<string, Dictionary<string, string>> _cachedTexts;
        private ObjectPool<Dictionary<string, string>> _cachedTextPool;

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            _loadedTables = new Dictionary<string, StringTable>(_maxTableCount);
            _cachedTexts = new Dictionary<string, Dictionary<string, string>>(_maxTableCount);

            _availableLanguages = LocalizationSettings.AvailableLocales.Locales.ToArray(l => new Language(l.Identifier.Code, l.LocaleName));
            _cachedTextPool = new ObjectPool<Dictionary<string, string>>(() => new Dictionary<string, string>(_cachedTextPerTableCapacity), 
                actionOnRelease: dict => dict.Clear(), collectionCheck: true, defaultCapacity: _maxTableCount);
        }

        public async UniTask Initialize()
        {
            await LocalizationSettings.InitializationOperation.ToUniTask();
            await UniTask.NextFrame(); // this prevents errors from calling localized strings the same frame initialization finishes

            PrewarmCachedTextDictionaries();
        }

        private void PrewarmCachedTextDictionaries()
        {
            var list = new List<Dictionary<string, string>>(_maxTableCount);

            for(int i = 0; i < _maxTableCount; i++)
            {
                list.Add(_cachedTextPool.Get());
            }

            for(int i = 0; i < _maxTableCount; i++)
            {
                _cachedTextPool.Release(list[i]);
            }

            list.Clear();
        }

        public Language[] GetLanguages()
        {
            return _availableLanguages;
        }

        public LocalizedText CreateLocalizedText(string tableId, string localizationId)
        {
            return new LocalizedText(tableId, localizationId);
        }

        public string GetLocalizedString(string tableId, string localizationId)
        {
            var tableIdEmpty = string.IsNullOrEmpty(tableId);
            var localizationIdEmpty = string.IsNullOrEmpty(localizationId);

            if(tableIdEmpty)
            {
                if(localizationIdEmpty)
                {
                    return string.Format(EMPTY_KEY_AND_TABLE);
                }

                return string.Format(EMPTY_TABLE, localizationId);
            }

            if(localizationIdEmpty)
            {
                return string.Format(EMPTY_KEY, tableId);
            }

            if(!IsTableLoaded(tableId))
            {
                return string.Format(NO_LOCALIZED_TABLE, localizationId, tableId);
            }

            return GetCachedOrAdd(tableId, localizationId);
        }

        private string GetCachedOrAdd(string tableId, string localizationId)
        {
            if(!_cachedTexts.TryGetValue(tableId, out var tableCache))
            {
                tableCache = _cachedTextPool.Get();
                _cachedTexts[tableId] = tableCache;
            }

            if(!tableCache.TryGetValue(localizationId, out var cachedText))
            {
                var table = _loadedTables[tableId];
                var entry = table.GetEntry(localizationId);
                if(entry == null)
                {
                    return string.Format(NO_LOCALIZED_KEY, localizationId, tableId);
                }

                cachedText = entry.GetLocalizedString();
                tableCache[localizationId] = cachedText;
            }

            return cachedText;
        }

        public Language GetTextLanguage()
        {
            var id = LocalizationSettings.SelectedLocale.Identifier.Code;
            var name = LocalizationSettings.SelectedLocale.LocaleName;

            return new Language(id, name);
        }

        public void SetTextLanguage(string languageId)
        {
            var locale = LocalizationSettings.AvailableLocales.GetLocale(languageId);

            if(LocalizationSettings.SelectedLocale == locale)
            {
                return;
            }

            LocalizationSettings.SelectedLocale = locale;

            ReloadOnLanguageChange();

            OnLanguageChanged?.Invoke(GetTextLanguage());
        }

        private void ReloadOnLanguageChange()
        {
            var tableIds = _loadedTables.Keys.ToArray();

            ClearCache();

            UnloadTables(tableIds);
            LoadTables(tableIds);
        }

        private void ClearCache()
        {
            foreach(var keyValue in _cachedTexts)
            {
                _cachedTextPool.Release(keyValue.Value);
            }

            _cachedTexts.Clear();
        }

        public bool IsTableLoaded(string tableId) => _loadedTables.ContainsKey(tableId);

        public async UniTask LoadTablesAsync(params string[] tableIds)
        {
            var finalTableIds = tableIds.Where(id => !IsTableLoaded(id)).ToArray();

            if(finalTableIds.Length == 0)
            {
                return;
            }

            var stringTables = await UniTask.WhenAll(finalTableIds.Select(id => LocalizationSettings.StringDatabase.GetTableAsync(id).ToUniTask()));

            for(int i = 0; i < stringTables.Length; i++)
            {
                _loadedTables[stringTables[i].TableCollectionName] = stringTables[i];
            }
        }

        public void LoadTables(params string[] tableIds)
        {
            var finalTableIds = tableIds.Where(id => !IsTableLoaded(id)).ToArray();

            if(finalTableIds.Length == 0)
            {
                return;
            }

            var stringTables = finalTableIds.Select(id => LocalizationSettings.StringDatabase.GetTable(id)).ToArray();

            for(int i = 0; i < stringTables.Length; i++)
            {
                _loadedTables[stringTables[i].TableCollectionName] = stringTables[i];
            }
        }

        public async UniTask UnloadTablesAsync(params string[] tableIds)
        {
            UnloadTables(tableIds);
        }

        public void UnloadTables(params string[] tableIds)
        {
            var finalTableIds = tableIds.Where(id => IsTableLoaded(id)).ToArray();

            if(finalTableIds.Length == 0)
            {
                return;
            }

            foreach(var tableId in finalTableIds)
            {
                _loadedTables.Remove(tableId);
                _cachedTexts.Remove(tableId);
            }

            for(int i = 0; i < finalTableIds.Length; i++)
            {
                LocalizationSettings.StringDatabase.ReleaseTable(finalTableIds[i]);
            }
        }
    }
}
