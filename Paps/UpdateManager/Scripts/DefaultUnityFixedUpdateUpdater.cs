using SaintsField.Playa;
using System;
using UnityEngine;

namespace Paps.UpdateManager
{
    public class DefaultUnityFixedUpdateUpdater : MonoBehaviour, IUpdater<IFixedUpdatable>
    {
        [SerializeField] private int _initialCapacity;
        
        [ShowInInspector, ListDrawerSettings(numberOfItemsPerPage: 10)]
        private FastRemoveList<IFixedUpdatable> _listeners;
        
        private bool HasListeners => _listeners.Count > 0;
        
        private void Awake()
        {
            _listeners = new FastRemoveList<IFixedUpdatable>(_initialCapacity);
            enabled = HasListeners;
        }
        
        public void Register(IFixedUpdatable listener)
        {
            _listeners.Add(listener);
            
            enabled = HasListeners;
        }

        public void Unregister(IFixedUpdatable listener)
        {
            _listeners.Remove(listener);

            enabled = HasListeners;
        }

        private void FixedUpdate()
        {
            foreach (IFixedUpdatable updatable in _listeners)
            {
                try
                {
                    updatable.ManagedFixedUpdate();
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
        }
    }
}