using System;
using UnityEngine.Profiling;

namespace Paps.UnityExtensions
{
    public class AllocCounter
    {
        private Recorder _recorder;

        public AllocCounter() {
            _recorder = Recorder.Get("GC.Alloc");
            _recorder.enabled = false;

#if !UNITY_WEBGL
            _recorder.FilterToCurrentThread();
#endif

            _recorder.enabled = true;
        }

        public int Stop() {
            if (_recorder == null) throw new InvalidOperationException("AllocCounter was not started.");

            _recorder.enabled = false;

#if !UNITY_WEBGL
            _recorder.CollectFromAllThreads();
#endif

            int result = _recorder.sampleBlockCount;
            _recorder = null;
            return result;
        }

        public static int Instrument(Action action) {
            var counter = new AllocCounter();
            int allocs;

            try {
                action();
            }
            finally {
                allocs = counter.Stop();
            }

            return allocs;
        }
    }
}