using Gilzoide.UpdateManager;
using System;
using UnityEngine;

namespace Paps.Timers
{
    public class SyncTimer : IUpdatable
    {
        public float Interval { get; set; }
        public bool Loop { get; set; }
        public event Action<SyncTimer> OnTick;
        public bool Active { get; private set; }

        private float _accumulationTime;

        public void Start()
        {
            if(Active)
                return;
            
            Active = true;
            _accumulationTime = 0;
            this.RegisterInManager();
        }

        public void Stop()
        {
            if(!Active)
                return;
            
            Active = false;
            this.UnregisterInManager();
        }
        
        public void ManagedUpdate()
        {
            _accumulationTime += Time.deltaTime;

            if (_accumulationTime >= Interval)
            {
                if(!Loop)
                    Stop();
                
                _accumulationTime = 0;
                OnTick?.Invoke(this);
            }
        }
    }
}
