using Paps.Time;
using Paps.UpdateManager;
using SaintsField.Playa;
using System;
using UnityEngine;
using UnityTime = UnityEngine.Time;

namespace Paps.ManualPhysicsSimulation
{
    public class ManualPhysicsFixedUpdateUpdater : MonoBehaviour, IUpdater<IFixedUpdatable>
    {
        [SerializeField] private int _initialCapacity;
        [SerializeField] private float _fixedTimeStep;
        [SerializeField] private TimeChannel _timeChannel;
        
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
    }
}
