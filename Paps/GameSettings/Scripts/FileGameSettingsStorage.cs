using Cysharp.Threading.Tasks;
using Paps.Logging;
using Paps.Persistence;
using SaintsField;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

namespace Paps.GameSettings
{
    public class FileGameSettingsStorage : ScriptableObject, IGameSettingsStorage
    {
        [SerializeField] private string _relativeFilePath;
        [SerializeField] private SaintsInterface<ISerializer> _serializer;

        public async UniTask<Dictionary<string, GameSettingSaveInfo>> Load(CancellationToken cancellationToken)
        {
            var absoluteFilePath = GetFilePath();

            if(!File.Exists(absoluteFilePath))
                return new Dictionary<string, GameSettingSaveInfo>();

            var serializedData = await File.ReadAllTextAsync(absoluteFilePath, cancellationToken);

            this.Log($"Loaded game settings:\n\n{serializedData}");

            return _serializer.I.Deserialize<Dictionary<string, GameSettingSaveInfo>>(serializedData);
        }

        public async UniTask Save(Dictionary<string, GameSettingSaveInfo> gameSettings)
        {
            EnsureDirectoryExists();

            var serializedData = _serializer.I.Serialize(gameSettings);

            this.Log($"Saving game settings:\n\n{serializedData}");

            await File.WriteAllTextAsync(GetFilePath(), serializedData);
        }

        public string GetFilePath() => Path.Combine(Application.persistentDataPath, _relativeFilePath);
        public string GetDirectory() => Path.GetDirectoryName(GetFilePath());

        private void EnsureDirectoryExists()
        {
            var directory = GetDirectory();

            if(!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }
    }
}
