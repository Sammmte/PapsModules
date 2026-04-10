using Cysharp.Threading.Tasks;
using SaintsField;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Paps.GameSettings
{
    public class GameSettingsManager : MonoBehaviour
    {
        public static GameSettingsManager Instance { get; private set; }

        [SerializeField] private SaintsInterface<IGameSettingsStorage> _storage;
        [SerializeField] private SaintsInterface<IDynamicGameSettingCreator>[] _dynamicGameSettingCreators;
        [SerializeField] private SaintsDictionary<string, GameSetting> _gameSettings;

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public async UniTask Initialize()
        {
            PrepareDynamicSettings();

            var saveInfoDictionary = await _storage.I.Load();

            foreach(var key in _gameSettings.Keys)
            {
                if(saveInfoDictionary.ContainsKey(key))
                {
                    _gameSettings[key].Initialize(saveInfoDictionary[key]);
                }
                else
                {
                    _gameSettings[key].Initialize();
                }
            }
        }

        private void PrepareDynamicSettings()
        {
            for(int i = 0; i < _dynamicGameSettingCreators.Length; i++)
            {
                var dynamicSettings = _dynamicGameSettingCreators[i].I.Create();

                if(dynamicSettings == null)
                    continue;

                for(int j = 0; j < dynamicSettings.Length; j++)
                {
                    var dynamicSettingKeyValue = dynamicSettings[j];
                    _gameSettings.Add(dynamicSettingKeyValue.Key, dynamicSettingKeyValue.Value);
                }
            }
        }

        public async UniTask Save()
        {
            if(!IsAnyDirty())
                return;

            CommitAll();

            await _storage.I.Save(CreateSaveDictionary());
        }

        private Dictionary<string, GameSettingSaveInfo> CreateSaveDictionary()
        {
            var saveDictionary = new Dictionary<string, GameSettingSaveInfo>(_gameSettings.Count);

            foreach(var keyValue in _gameSettings)
            {
                var maybeSaveInfo = keyValue.Value.GetSaveInfo();

                if(maybeSaveInfo.HasValue)
                {
                    saveDictionary.Add(keyValue.Key, maybeSaveInfo);
                }
            }

            return saveDictionary;
        }

        public GameSetting GetSetting(string id)
        {
            if(!_gameSettings.ContainsKey(id))
                return null;

            return _gameSettings[id];
        }

        public GameSetting<T> GetSetting<T>(string id) where T : IEquatable<T>
        {
            var setting = GetSetting(id);
            
            if(setting != null && setting is GameSetting<T> casted)
            {
                return casted;
            }

            return null;
        }

        public List<T> GetSettings<T>() where T : GameSetting
        {
            var list = new List<T>(_gameSettings.Count);

            foreach(var keyValue in _gameSettings)
            {
                if(keyValue.Value is T casted)
                {
                    list.Add(casted);
                }
            }

            return list;
        }

        public bool IsAnyDirty()
        {
            foreach(var keyValue in _gameSettings)
            {
                if(keyValue.Value.IsDirty)
                    return true;
            }

            return false;
        }

        public void ResetAll()
        {
            foreach(var keyValue in _gameSettings)
            {
                keyValue.Value.Reset();
            }
        }

        public void ResetToDefault()
        {
            foreach(var keyValue in _gameSettings)
            {
                keyValue.Value.ResetToDefault();
            }
        }

        private void CommitAll()
        {
            foreach(var keyValue in _gameSettings)
            {
                keyValue.Value.CommitChange();
            }
        }
    }
}
