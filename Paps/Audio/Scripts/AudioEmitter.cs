using Gilzoide.UpdateManager;
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
            _audioSource.clip = audioParameters.AudioClip;
            _audioSource.outputAudioMixerGroup = audioParameters.AudioMixerGroup;
            _audioSource.volume = audioParameters.Volume;
            _audioSource.spatialBlend = audioParameters.SpatialBlend;
            _audioSource.loop = audioParameters.Loop;
            transform.position = audioParameters.Position;
            
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
    }
}