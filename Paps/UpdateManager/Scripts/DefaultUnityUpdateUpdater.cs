using SaintsField.Playa;
using System;
using UnityEngine;

namespace Paps.UpdateManager
{
    public class DefaultUnityUpdateUpdater : MonoBehaviour, IUpdater<IUpdatable>
    {
        [SerializeField] private int _initialCapacity;
        
        [ShowInInspector, ListDrawerSettings(numberOfItemsPerPage: 10)]
        private FastRemoveList<IUpdatable> _listeners;
        
        private bool HasListeners => _listeners.Count > 0;

        private void Awake()
        {
            _listeners = new FastRemoveList<IUpdatable>(_initialCapacity);
            enabled = HasListeners;
        }

        public void Register(IUpdatable listener)
        {
            _listeners.Add(listener);
            
            enabled = HasListeners;
        }

        public void Unregister(IUpdatable listener)
        {
            _listeners.Remove(listener);

            enabled = HasListeners;
        }

        private void Update()
        {
            foreach (IUpdatable updatable in _listeners)
            {
                try
                {
                    updatable.ManagedUpdate();
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
        }
        
        // DISPOSE?
    }
}