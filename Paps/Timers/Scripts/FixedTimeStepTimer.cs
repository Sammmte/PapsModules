
using Paps.UpdateManager;
using System;

namespace Paps.Timers
{
    public class FixedTimeStepTimer : IFixedUpdatable
    {
        public int Steps { get; set; }
        public bool Loop { get; set; }
        public event Action<FixedTimeStepTimer> OnTick;
        public bool Active { get; private set; }

        private float _accumulationFrames;

        public void Start()
        {
            if(Active)
                return;
            
            Active = true;
            _accumulationFrames = 0;
            this.RegisterFixedUpdate();
        }

        public void Stop()
        {
            if(!Active)
                return;
            
            Active = false;
            this.UnregisterFixedUpdate();
        }
        
        public void ManagedFixedUpdate()
        {
            _accumulationFrames++;

            if (_accumulationFrames >= Steps)
            {
                if(!Loop)
                    Stop();
                
                _accumulationFrames = 0;
                OnTick?.Invoke(this);
            }
        }
    }
}