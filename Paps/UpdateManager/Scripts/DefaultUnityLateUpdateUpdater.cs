using Paps.ValueReferences;
using SaintsField;
using SaintsField.Playa;
using System;
using UnityEngine;

namespace Paps.UpdateManager
{
    public class DefaultUnityLateUpdateUpdater : MonoBehaviour, IUpdater<ILateUpdatable>
    {
        [AboveButton(nameof(Enable), "Enable")]
        [AboveButton(nameof(Disable), "Disable")]
        [SerializeField] private int _initialCapacity;
        [SerializeField] private ValueReference<int> _id;
        
        [ShowInInspector, ListDrawerSettings(numberOfItemsPerPage: 10)]
        private FastRemoveList<ILateUpdatable> _listeners;
        
        private bool HasListeners => _listeners.Count > 0;
        public int Id => _id;
        [NonSerialized, ShowInInspector, ReadOnly] private bool _manualEnabled;

        private void Awake()
        {
            _listeners = new FastRemoveList<ILateUpdatable>(_initialCapacity);
            _manualEnabled = enabled;
            UpdateEnabled();
        }

        public void Register(ILateUpdatable listener)
        {
            _listeners.Add(listener);
            
            UpdateEnabled();
        }

        public void Unregister(ILateUpdatable listener)
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

        private void UpdateEnabled()
        {
            enabled = HasListeners && _manualEnabled;
        }
    }
}