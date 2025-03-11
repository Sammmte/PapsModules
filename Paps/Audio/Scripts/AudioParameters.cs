using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Paps.Audio
{
    [Serializable]
    public struct AudioParameters
    {
        [SerializeField] public AudioClip AudioClip;
        [SerializeField] public AudioMixerGroup AudioMixerGroup;
        [SerializeField] [Range(0f, 1f)] public float Volume;
        [SerializeField] [Range(0f, 1f)] public float SpatialBlend;
        [SerializeField] public Vector3 Position;
        [SerializeField] public bool Loop;
        
        public AudioParameters(AudioClip clip, AudioMixerGroup audioMixerGroup)
        {
            AudioClip = clip;
            AudioMixerGroup = audioMixerGroup;
            Volume = 1;
            SpatialBlend = 0;
            Position = Vector3.zero;
            Loop = false;
        }
    }
}