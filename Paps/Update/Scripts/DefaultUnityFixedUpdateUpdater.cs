using Paps.ValueReferences;
using SaintsField;
using SaintsField.Playa;
using System;
using UnityEngine;

namespace Paps.Update
{
    public class DefaultUnityFixedUpdateUpdater : MonoBehaviour, IUpdater<IFixedUpdatable>
    {
        [AboveButton(nameof(Enable), "Enable")]
        [AboveButton(nameof(Disable), "Disable")]
        [SerializeField] private int _initialCapacity;
        [SerializeField] private ValueReference<int> _id;
        
        [ShowInInspector, ListDrawerSettings(numberOfItemsPerPage: 10)]
        private FastRemoveList<IFixedUpdatable> _listeners;
        
        private bool HasListeners => _listeners.Count > 0;
        public int Id => _id;
        [NonSerialized, ShowInInspector, ReadOnly] private bool _manualEnabled;
        
        private void Awake()
        {
            _listeners = new FastRemoveList<IFixedUpdatable>(_initialCapacity);
            _manualEnabled = enabled;
            UpdateEnabled();
        }

        public void Register(IFixedUpdatable listener)
        {
            _listeners.Add(listener);
            
            UpdateEnabled();
        }

        public void Unregister(IFixedUpdatable listener)
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

        private void UpdateEnabled()
        {
            enabled = HasListeners && _manualEnabled;
        }
    }
}