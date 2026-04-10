using SaintsField;
using System;
using UnityEngine;

namespace Paps.GameSettings
{
    [Serializable]
    public class GameSettingsStorageProvider
    {
        [SerializeField] private SaintsInterface<IGameSettingsStorage> _actualStorage;

        private InMemoryGameSettingsStorage _inMemoryStorage = new InMemoryGameSettingsStorage();

        public IGameSettingsStorage Storage
        {
            get
            {
                if(Persistence.Persistence.PersistenceEnabled)
                    return _actualStorage.I;

                return _inMemoryStorage;
            }
        }
    }
}
