using Paps.Optionals;
using Paps.Update;
using SaintsField;
using System;
using UnityEngine;

namespace Paps.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioEmitter : MonoBehaviour, IUpdatable
    {
        [SerializeField, GetComponent(typeof(AudioSource))] private AudioSource _audioSource;
        
        public event Action<AudioEmitter> OnStopped;

        public bool IsPlaying => _audioSource.isPlaying;

        public void Play(AudioParameters audioParameters)
        {
            _audioSource.clip = audioParameters.AudioClip.ValueOrDefault(_audioSource.clip);
            _audioSource.outputAudioMixerGroup = audioParameters.AudioMixerGroup.ValueOrDefault(_audioSource.outputAudioMixerGroup);
            _audioSource.volume = audioParameters.Volume.ValueOrDefault(_audioSource.volume);
            _audioSource.spatialBlend = audioParameters.SpatialBlend.ValueOrDefault(_audioSource.spatialBlend);
            _audioSource.loop = audioParameters.Loop.ValueOrDefault(_audioSource.loop);
            transform.position = audioParameters.Position.ValueOrDefault(transform.position);
            _audioSource.pitch = audioParameters.Pitch.ValueOrDefault(_audioSource.pitch);
            
            Play();
        }

        public void Play()
        {
            if(_audioSource.isPlaying)
                return;

            _audioSource.Play();
            
            this.RegisterUpdate();
        }

        public void Stop()
        {
            if(!_audioSource.isPlaying)
                return;

            this.UnregisterUpdate();
            
            _audioSource.Stop();
            OnStopped?.Invoke(this);
        }

        void IUpdatable.ManagedUpdate()
        {
            if(!_audioSource.isPlaying)
                Stop();
        }

        private void OnDestroy()
        {
            Stop();
        }
    }
}