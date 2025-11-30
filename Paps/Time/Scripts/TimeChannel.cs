using SaintsField.Playa;
using System;
using UnityEngine;

namespace Paps.Time
{
    [CreateAssetMenu(menuName = "Paps/Time/Time Channel")]
    public class TimeChannel : ScriptableObject
    {
        [field: SerializeField] public string Id { get; private set; }
        [field: SerializeField] private float _initialTimeScale = 1f;

        [NonSerialized] private float _timeScale;
        [NonSerialized] private float _maxAllowedDeltaTime;

        [ShowInInspector]
        public float TimeScale
        {
            get => _timeScale;
            set
            {
                if (Mathf.Approximately(_timeScale, value)) return;

                _timeScale = value;
                OnTimeScaleChanged?.Invoke(this, _timeScale);
            }
        }
        [ShowInInspector] public float TimeSinceStart { get; private set; }
        [ShowInInspector] public float DeltaTime { get; private set; }
        [ShowInInspector] public float UnscaledTimeSinceStart { get; private set; }
        [ShowInInspector] public float UnscaledDeltaTime { get; private set; }

        [NonSerialized] private bool _paused;

        public bool Paused
        {
            get => _paused;
            set
            {
                if (_paused == value) return;

                _paused = value;
                if (_paused)
                    OnPaused?.Invoke(this);
                else
                    OnUnpaused?.Invoke(this);
            }
        }
        
        public event Action<TimeChannel, float> OnTimeScaleChanged;
        public event Action<TimeChannel> OnPaused; 
        public event Action<TimeChannel> OnUnpaused; 

        internal void Initialize(TimeChannelConfiguration configuration)
        {
            TimeScale = _initialTimeScale;

            _maxAllowedDeltaTime = configuration.MaxAllowedTimeStep;
        }

        internal void UpdateChannel(float unscaledDelta, float globalTimeScale)
        {
            UnscaledDeltaTime = unscaledDelta;
            UnscaledTimeSinceStart += unscaledDelta;

            float effectiveScale = TimeScale * globalTimeScale;
            DeltaTime = Paused ? 0f : Mathf.Min(unscaledDelta * effectiveScale, _maxAllowedDeltaTime);

            if (!Paused) TimeSinceStart += DeltaTime;
        }
    }
}