using System;

namespace Paps.Timers
{
    public class PerFramesCheck<TCheckResult>
    {
        private readonly Func<TCheckResult> _check;
        private readonly FrameTimer _timer = new FrameTimer();

        public int Frames
        {
            get => _timer.Frames;
            set => _timer.Frames = value;
        }

        public TCheckResult LastResult { get; private set; }

        public PerFramesCheck(Func<TCheckResult> check)
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