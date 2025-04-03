using System;

namespace Paps.Timers
{
    public class TimedCheck<TCheckResult>
    {
        private readonly Func<TCheckResult> _check;
        private readonly SyncTimer _timer = new SyncTimer();

        public float Interval
        {
            get => _timer.Interval;
            set => _timer.Interval = value;
        }

        private TCheckResult _lastResult;

        public TimedCheck(Func<TCheckResult> check)
        {
            _check = check;
        }

        public TCheckResult Check()
        {
            if (!_timer.Active)
            {
                _timer.Start();
                _lastResult = _check();
            }

            return _lastResult;
        }

        public void Stop()
        {
            _timer.Stop();
        }
    }
}