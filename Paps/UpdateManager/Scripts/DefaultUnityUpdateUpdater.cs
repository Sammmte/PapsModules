using Paps.ValueReferences;
using SaintsField;
using SaintsField.Playa;
using System;
using UnityEngine;

namespace Paps.UpdateManager
{
    public class DefaultUnityUpdateUpdater : MonoBehaviour, IUpdater<IUpdatable>
    {
        [AboveButton(nameof(Enable), "Enable")]
        [AboveButton(nameof(Disable), "Disable")]
        [SerializeField] private int _initialCapacity;
        [SerializeField] private ValueReference<int> _id;
        
        [ShowInInspector, ListDrawerSettings(numberOfItemsPerPage: 10)]
        private FastRemoveList<IUpdatable> _listeners;
        
        private bool HasListeners => _listeners.Count > 0;
        public int Id => _id;
        [NonSerialized, ShowInInspector, ReadOnly] private bool _manualEnabled;

        private void Awake()
        {
            _listeners = new FastRemoveList<IUpdatable>(_initialCapacity);
            _manualEnabled = enabled;
            UpdateEnabled();
        }

        public void Register(IUpdatable listener)
        {
            _listeners.Add(listener);
            
            UpdateEnabled();
        }

        public void Unregister(IUpdatable listener)
        {
            _listeners.Remove(listener);

            UpdateEnabled();
        }

        public void Enable()
        {
            _manualEnabled = true;
            
            UpdateEnabled();
        }

        public void Disable()
        {
            _manualEnabled = false;
            
            UpdateEnabled();
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

        private void UpdateEnabled()
        {
            enabled = HasListeners && _manualEnabled;
        }
    }
}