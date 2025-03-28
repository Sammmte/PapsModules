using Gilzoide.UpdateManager;
using Paps.Optionals;
using System;
using UnityEngine;

namespace Paps.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioEmitter : MonoBehaviour, IUpdatable
    {
        [SerializeField, HideInInspector] private AudioSource _audioSource;
        
        internal event Action<AudioEmitter> OnStopped;

        internal void Play(AudioParameters audioParameters)
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

        internal void Play()
        {
            _audioSource.Play();
            
            this.RegisterInManager();
        }

        internal void Stop()
        {
            this.UnregisterInManager();
            
            _audioSource.Stop();
            OnStopped?.Invoke(this);
        }
        
        private void OnValidate()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        void IUpdatable.ManagedUpdate()
        {
            if(!_audioSource.isPlaying)
                Stop();
        }

        private void OnDestroy()
        {
            this.UnregisterInManager();
            
            _audioSource.Stop();
        }
    }
}