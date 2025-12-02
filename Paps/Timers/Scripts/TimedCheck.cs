using Paps.Optionals;
using Paps.Time;

namespace Paps.Timers
{
    public class TimedCheck<TCheckResult>
    {
        public delegate bool CheckPredicate(out TCheckResult result);
        
        private readonly CheckPredicate _check;
        private readonly SyncTimer _timer = new SyncTimer();

        public TimeChannel TimeChannel
        {
            get => _timer.TimeChannel;
            set => _timer.TimeChannel = value;
        }

        public float Interval
        {
            get => _timer.Interval;
            set => _timer.Interval = value;
        }

        public Optional<int> UpdateUpdaterId
        {
            get => _timer.UpdateUpdaterId;
            set => _timer.UpdateUpdaterId = value;
        }
        
        public bool UseTimeManager
        {
            get => _timer.UseTimeManager;
            set => _timer.UseTimeManager = value;
        }

        public TimedCheck(CheckPredicate check)
        {
            _check = check;
        }

        public bool Check(out TCheckResult result)
        {
            if (!_timer.Active)
            {
                _timer.Start();
                return _check(out result);
            }

            result = default;
            return false;
        }

        public void Stop()
        {
            _timer.Stop();
        }
    }
}