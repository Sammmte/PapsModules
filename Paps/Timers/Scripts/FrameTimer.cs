using Paps.Optionals;
using Paps.UpdateManager;
using System;

namespace Paps.Timers
{
    public class FrameTimer : IUpdatable
    {
        public int Frames { get; set; }
        public bool Loop { get; set; }
        public event Action<FrameTimer> OnTick;
        public bool Active { get; private set; }
        
        private Optional<int> _updateUpdaterId;

        public Optional<int> UpdateUpdaterId
        {
            get => _updateUpdaterId;
            set
            {
                if (Active)
                {
                    Unregister();
                    _updateUpdaterId = value;
                    Register();
                }
                else
                {
                    _updateUpdaterId = value;
                }
            }
        }

        private float _accumulationFrames;

        public void Start()
        {
            if(Active)
                return;
            
            Active = true;
            _accumulationFrames = 0;
            Register();
        }

        public void Stop()
        {
            if(!Active)
                return;
            
            Active = false;
            Unregister();
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
        
        private void Register()
        {
            if (UpdateUpdaterId.HasValue)
            {
                this.RegisterUpdate(UpdateUpdaterId);
            }
            else
            {
                this.RegisterUpdate();
            }
        }

        private void Unregister()
        {
            if (UpdateUpdaterId.HasValue)
            {
                this.UnregisterUpdate(UpdateUpdaterId);
            }
            else
            {
                this.UnregisterUpdate();
            }
        }
    }
}