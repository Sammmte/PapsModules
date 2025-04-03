using Gilzoide.UpdateManager;
using System;

namespace Paps.Timers
{
    public class FrameTimer : IUpdatable
    {
        public float Frames { get; set; }
        public bool Loop { get; set; }
        public event Action<FrameTimer> OnTick;
        public bool Active { get; private set; }

        private float _accumulationFrames;

        public void Start()
        {
            if(Active)
                return;
            
            Active = true;
            _accumulationFrames = 0;
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
            _accumulationFrames++;

            if (_accumulationFrames >= Frames)
            {
                if(!Loop)
                    Stop();
                
                _accumulationFrames = 0;
                OnTick?.Invoke(this);
            }
        }
    }
}