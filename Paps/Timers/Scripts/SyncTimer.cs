using Paps.Time;
using Paps.UpdateManager;
using System;

namespace Paps.Timers
{
    public class SyncTimer : IUpdatable
    {
        public float Interval { get; set; }
        public bool Loop { get; set; }
        public event Action<SyncTimer> OnTick;
        public bool Active { get; private set; }
        public bool Paused { get; set; }
        public TimeChannel TimeChannel { get; set; }

        private float _accumulationTime;

        public void Start()
        {
            if(Active)
                return;
            
            Active = true;
            Paused = false;
            _accumulationTime = 0;
            this.RegisterUpdate();
        }
        
        public void Restart()
        {
            if (Active)
            {
                Paused = false;
                _accumulationTime = 0;
            }
            else
            {
                Start();
            }
        }

        public void Stop()
        {
            if(!Active)
                return;
            
            Active = false;
            Paused = false;
            this.UnregisterUpdate();
        }
        
        public void ManagedUpdate()
        {
            if(Paused)
                return;

            _accumulationTime += GetDeltaTime();

            if (_accumulationTime >= Interval)
            {
                if(!Loop)
                    Stop();
                
                _accumulationTime = 0;
                OnTick?.Invoke(this);
            }
        }
        
        private float GetDeltaTime()
        {
            if (TimeChannel != null)
                return TimeChannel.DeltaTime;
            return TimeManager.Instance.GlobalDeltaTime;
        }
    }
}
