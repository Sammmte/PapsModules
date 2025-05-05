using System;

namespace Paps.Timers
{
    public class PerFixedTimeStepCheck<TCheckResult>
    {
        private readonly Func<TCheckResult> _check;
        private readonly FixedTimeStepTimer _timer = new FixedTimeStepTimer();

        public int Steps
        {
            get => _timer.Steps;
            set => _timer.Steps = value;
        }

        public TCheckResult LastResult { get; private set; }

        public PerFixedTimeStepCheck(Func<TCheckResult> check)
        {
            _check = check;
        }

        public TCheckResult Check()
        {
            if (!_timer.Active)
            {
                _timer.Start();
                LastResult = _check();
            }

            return LastResult;
        }

        public void Stop()
        {
            _timer.Stop();
        }
    }
}