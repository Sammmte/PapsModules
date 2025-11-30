using SaintsField.Playa;
using System.Collections.Generic;
using UnityEngine;
using UnityTime = UnityEngine.Time;

namespace Paps.Time
{
    [DefaultExecutionOrder(-10000)]
    public class TimeManager : MonoBehaviour
    {
        public static TimeManager Instance { get; private set; }

        [field: SerializeField] public float GlobalTimeScale { get; set; } = 1f;
        [field: SerializeField] public float MaxAllowedTimeStep { get; private set; } = 0.333333f;
        [ShowInInspector] public float GlobalTimeSinceStart { get; private set; }
        [ShowInInspector] public float GlobalDeltaTime { get; private set; }
        [ShowInInspector] public float GlobalUnscaledTimeSinceStart => UnityTime.unscaledTime;
        [ShowInInspector] public float GlobalUnscaledDeltaTime => UnityTime.unscaledDeltaTime;
        [SerializeField] private TimeChannel[] _timeChannels;

        private Dictionary<string, TimeChannel> _channels;

        void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            _channels = new Dictionary<string, TimeChannel>(_timeChannels.Length);
            var configuration = new TimeChannelConfiguration
            {
                MaxAllowedTimeStep = MaxAllowedTimeStep
            };
            
            for(int i = 0; i < _timeChannels.Length; i++)
            {
                var channel = _timeChannels[i];
                channel.Initialize(configuration);
                _channels[channel.Id] = channel;
            }
        }

        void Update()
        {
            float unscaledDelta = UnityTime.unscaledDeltaTime;
            
            GlobalDeltaTime = unscaledDelta * GlobalTimeScale;
            GlobalTimeSinceStart += GlobalDeltaTime;

            for (int i = 0; i < _timeChannels.Length; i++)
            {
                _timeChannels[i].UpdateChannel(unscaledDelta, GlobalTimeScale);
            }
        }

        public TimeChannel GetChannel(string id)
        {
            _channels.TryGetValue(id, out var channel);
            return channel;
        }

        public void PauseAll()
        {
            for(int i = 0; i < _timeChannels.Length; i++)
            {
                _timeChannels[i].Paused = true;
            }
        }
        
        public void UnpauseAll()
        {
            for(int i = 0; i < _timeChannels.Length; i++)
            {
                _timeChannels[i].Paused = false;
            }
        }
    }
}
