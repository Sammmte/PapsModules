using System;
using UnityEngine;

namespace Paps.Persistence
{
    public static class Persistence
    {
        private static bool _persistenceEnabled;
        public static bool PersistenceEnabled
        {
            get => _persistenceEnabled;
            set
            {
                var previousValue = _persistenceEnabled;

                _persistenceEnabled = value;

                if(previousValue != _persistenceEnabled)
                {
                    OnPersistenceStatusChanged?.Invoke();
                }
            }
        }

        public static event Action OnPersistenceStatusChanged;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        public static void Initialize()
        {
            PersistenceEnabled = 
#if PERSISTENCE
            true
#else
            false
#endif
            ;
        }
    }
}