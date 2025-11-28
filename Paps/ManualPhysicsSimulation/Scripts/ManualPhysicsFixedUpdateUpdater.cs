using Paps.UpdateManager;
using SaintsField.Playa;
using System;
using UnityEngine;

namespace Paps.ManualPhysicsSimulation
{
    public class ManualPhysicsFixedUpdateUpdater : MonoBehaviour, IUpdater<IFixedUpdatable>
    {
        [SerializeField] private int _initialCapacity;
        
        [ShowInInspector, ListDrawerSettings(numberOfItemsPerPage: 10)]
        private FastRemoveList<IFixedUpdatable> _listeners;
        
        private float _accumulatedDeltaTime;
        
        private bool HasListeners => _listeners.Count > 0;
        
        private void Awake()
        {
            _listeners = new FastRemoveList<IFixedUpdatable>(_initialCapacity);
            enabled = HasListeners;

            Physics.simulationMode = SimulationMode.Script;
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

        void Update()
        {
            _accumulatedDeltaTime += Time.deltaTime;

            while (_accumulatedDeltaTime >= Time.fixedDeltaTime)
            {
                _accumulatedDeltaTime -= Time.fixedDeltaTime;
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
                Physics.Simulate(Time.fixedDeltaTime);
            }

            enabled = HasListeners;
        }
    }
}
