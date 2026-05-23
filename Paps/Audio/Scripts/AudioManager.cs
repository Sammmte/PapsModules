using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;

namespace Paps.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [field: SerializeField] public AudioMixer AudioMixer { get; private set; }
        [field: SerializeField] private AudioEmitter _audioEmitterPrefab;
        [SerializeField] private int _poolCapacity;
        [SerializeField] private bool _prewarmPool;
        
        private Transform _audioEmittersParent;
        private ObjectPool<AudioEmitter> _audioEmitterPool;
        private List<AudioEmitter> _activePooledAudioEmitters;

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            _audioEmittersParent = new GameObject("AudioEmittersParent").transform;
            GameObject.DontDestroyOnLoad(_audioEmittersParent.gameObject);
            _audioEmitterPool = new ObjectPool<AudioEmitter>(CreateAudioEmitter, OnGetAudioEmitter, OnReleaseAudioEmitter,
                defaultCapacity: _poolCapacity, collectionCheck: true);

            _activePooledAudioEmitters = new List<AudioEmitter>(_poolCapacity);

            if(_prewarmPool)
                PrewarmPool();
        }

        private void PrewarmPool()
        {
            for(int i = 0; i < _poolCapacity; i++)
            {
                _activePooledAudioEmitters.Add(_audioEmitterPool.Get());
            }

            for(int i = 0; i < _poolCapacity; i++)
            {
                _audioEmitterPool.Release(_activePooledAudioEmitters[i]);
            }

            _activePooledAudioEmitters.Clear();
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

        private void OnDestroy()
        {
            _audioEmitterPool.Dispose();
        }
    }
}
