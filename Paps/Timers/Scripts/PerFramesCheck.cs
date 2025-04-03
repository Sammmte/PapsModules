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

        private TCheckResult _lastResult;

        public PerFramesCheck(Func<TCheckResult> check)
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