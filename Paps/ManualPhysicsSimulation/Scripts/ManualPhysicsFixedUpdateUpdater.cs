using Paps.Time;
using Paps.UpdateManager;
using Paps.ValueReferences;
using SaintsField.Playa;
using System;
using UnityEngine;
using UnityTime = UnityEngine.Time;

namespace Paps.ManualPhysicsSimulation
{
    public class ManualPhysicsFixedUpdateUpdater : MonoBehaviour, IUpdater<IFixedUpdatable>
    {
        [SerializeField] private ValueReference<int> _id;
        [SerializeField] private int _initialCapacity;
        [SerializeField] private float _fixedTimeStep;
        [SerializeField] private TimeChannel _timeChannel;
        
        [ShowInInspector, ListDrawerSettings(numberOfItemsPerPage: 10)]
        private FastRemoveList<IFixedUpdatable> _listeners;
        
        private float _accumulatedDeltaTime;
        
        private bool HasListeners => _listeners.Count > 0;
        public int Id => _id;
        private bool _manualEnabled;
        
        private void Awake()
        {
            _listeners = new FastRemoveList<IFixedUpdatable>(_initialCapacity);
            _manualEnabled = enabled;
            
            UpdateEnabled();

            Physics.simulationMode = SimulationMode.Script;
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

        void Update()
        {
            var deltaTime = GetDeltaTime();
            var paused = _timeChannel?.Paused ?? false;
            
            if(deltaTime == 0 || paused)
                return;

            _accumulatedDeltaTime += deltaTime;
            var timeStep = _fixedTimeStep * GetTimeScale();

            while (_accumulatedDeltaTime >= timeStep)
            {
                _accumulatedDeltaTime -= timeStep;
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
                Physics.Simulate(timeStep);
            }

            enabled = HasListeners;
        }
        
        private float GetDeltaTime() => _timeChannel?.DeltaTime ?? UnityTime.deltaTime;
        private float GetTimeScale() => _timeChannel?.TimeScale ?? UnityTime.timeScale;

        private void UpdateEnabled()
        {
            enabled = HasListeners && _manualEnabled;
        }
    }
}
