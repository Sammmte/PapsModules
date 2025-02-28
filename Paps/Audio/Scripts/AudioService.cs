using UnityEngine.Audio;

namespace Paps.Audio
{
    public class AudioService
    {
        private AudioMixer _audioMixer;

        public AudioService(AudioMixer audioMixer)
        {
            _audioMixer = audioMixer;
        }

        public float MasterVolume { 
            get
            {
                _audioMixer.GetFloat("MasterVolume", out float value);

                return value;
            }
            set => _audioMixer.SetFloat("MasterVolume", value);
        }
        public float MusicVolume
        {
            get
            {
                _audioMixer.GetFloat("MusicVolume", out float value);

                return value;
            }
            set => _audioMixer.SetFloat("MusicVolume", value);
        }
        public float SFXVolume
        {
            get
            {
                _audioMixer.GetFloat("SFXVolume", out float value);

                return value;
            }
            set => _audioMixer.SetFloat("SFXVolume", value);
        }
    }
}
