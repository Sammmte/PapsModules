using Cysharp.Threading.Tasks;
using Paps.GameSetup;
using Paps.Persistence;
using SaintsField;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Paps.GameSettings
{
    public class GameSettingsManager : MonoBehaviour, IPreGameSetupInitialization
    {
        public static GameSettingsManager Instance { get; private set; }

        [SerializeField] private GameSettingsStorageProvider _storageProvider;
        [SerializeField] private SaintsInterface<IDynamicGameSettingCreator>[] _dynamicGameSettingCreators;
        [SerializeField] private SaintsDictionary<string, GameSetting> _gameSettings;

        private DataStorageReader _reader = new DataStorageReader();
        private DataStorageWriter _writer = new DataStorageWriter();

        public void PreGameSetupInitialize()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public async UniTask Initialize(CancellationToken cancellationToken = default)
        {
            var dataStorage = await _storageProvider.Storage.Load(cancellationToken);

            PrepareDynamicSettings();

            if(dataStorage == null)
                dataStorage = DataStorage<string>.Rent();

            foreach(var settingId in _gameSettings.Keys)
            {
                _reader.Prepare(settingId, dataStorage);
                _gameSettings[settingId].Initialize(_reader);
            }

            dataStorage.Return();
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

            var dataStorage = GetSaveDataStorage();

            await _storageProvider.Storage.Save(dataStorage);

            dataStorage.Return();
        }

        private DataStorage<string> GetSaveDataStorage()
        {
            var dataStorage = DataStorage<string>.Rent();

            foreach(var keyValue in _gameSettings)
            {
                if(keyValue.Value.IsDefault)
                    continue;

                _writer.Prepare(keyValue.Key, dataStorage);

                keyValue.Value.Save(_writer);
            }

            return dataStorage;
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
