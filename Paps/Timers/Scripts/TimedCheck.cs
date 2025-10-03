using System;

namespace Paps.Timers
{
    public class TimedCheck<TCheckResult>
    {
        public delegate bool CheckPredicate(out TCheckResult result);
        
        private readonly CheckPredicate _check;
        private readonly SyncTimer _timer = new SyncTimer();

        public float Interval
        {
            get => _timer.Interval;
            set => _timer.Interval = value;
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