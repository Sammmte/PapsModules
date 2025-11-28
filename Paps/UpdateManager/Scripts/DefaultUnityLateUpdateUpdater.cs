using SaintsField.Playa;
using System;
using UnityEngine;

namespace Paps.UpdateManager
{
    public class DefaultUnityLateUpdateUpdater : MonoBehaviour, IUpdater<ILateUpdatable>
    {
        [SerializeField] private int _initialCapacity;
        
        [ShowInInspector, ListDrawerSettings(numberOfItemsPerPage: 10)]
        private FastRemoveList<ILateUpdatable> _listeners;
        
        private bool HasListeners => _listeners.Count > 0;
        
        private void Awake()
        {
            _listeners = new FastRemoveList<ILateUpdatable>(_initialCapacity);
            enabled = HasListeners;
        }
        
        public void Register(ILateUpdatable listener)
        {
            _listeners.Add(listener);
            
            enabled = HasListeners;
        }

        public void Unregister(ILateUpdatable listener)
        {
            _listeners.Remove(listener);

            enabled = HasListeners;
        }

        private void LateUpdate()
        {
            foreach (ILateUpdatable updatable in _listeners)
            {
                try
                {
                    updatable.ManagedLateUpdate();
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
        }
    }
}