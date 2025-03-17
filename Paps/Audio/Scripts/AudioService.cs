using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;

namespace Paps.Audio
{
    public class AudioService
    {
        public AudioMixer AudioMixer { get; }
        
        private Transform _audioEmittersParent;
        private AudioEmitter _audioEmitterPrefab;
        private ObjectPool<AudioEmitter> _audioEmitterPool;
        private List<AudioEmitter> _activePooledAudioEmitters = new List<AudioEmitter>();

        public AudioService(AudioMixer audioMixer, AudioEmitter audioEmitterPrefab)
        {
            AudioMixer = audioMixer;
            _audioEmitterPrefab = audioEmitterPrefab;
            _audioEmittersParent = new GameObject("AudioEmittersParent").transform;
            GameObject.DontDestroyOnLoad(_audioEmittersParent.gameObject);
            _audioEmitterPool = new ObjectPool<AudioEmitter>(CreateAudioEmitter, OnGetAudioEmitter, OnReleaseAudioEmitter);
        }

        private AudioEmitter CreateAudioEmitter()
        {
            var newAudioEmitter = GameObject.Instantiate(_audioEmitterPrefab, _audioEmittersParent);
            newAudioEmitter.OnStopped += Release;
            newAudioEmitter.gameObject.SetActive(false);
            return newAudioEmitter;
        }

        private void Release(AudioEmitter audioEmitter) => _audioEmitterPool.Release(audioEmitter);

        private void OnGetAudioEmitter(AudioEmitter audioEmitter)
        {
            _activePooledAudioEmitters.Add(audioEmitter);
            audioEmitter.gameObject.SetActive(true);
        }

        private void OnReleaseAudioEmitter(AudioEmitter audioEmitter)
        {
            _activePooledAudioEmitters.Remove(audioEmitter);
            audioEmitter.gameObject.SetActive(false);
        }

        public AudioEmitter Play(AudioParameters audioParameters)
        {
            var audioEmitter = _audioEmitterPool.Get();
            audioEmitter.Play(audioParameters);
            return audioEmitter;
        }
        
        public void Play(AudioEmitter audioEmitter, AudioParameters audioParameters) => audioEmitter.Play(audioParameters);

        public void Play(AudioEmitter audioEmitter) => audioEmitter.Play();

        public void Stop(AudioEmitter audioEmitter)
        {
            audioEmitter.Stop();
        }
    }
}
