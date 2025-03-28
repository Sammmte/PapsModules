using Paps.Optionals;
using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Paps.Audio
{
    [Serializable]
    public struct AudioParameters
    {
        [SerializeField] private Optional<AudioClip> _audioClip;
        [SerializeField] private Optional<AudioMixerGroup> _audioMixerGroup;
        [SerializeField] private Optional<float> _volume;
        [SerializeField] private Optional<float> _spatialBlend;
        [SerializeField] private Optional<Transform> _positionTransform;
        [SerializeField] private Optional<Vector3> _position;
        [SerializeField] private Optional<bool> _loop;
        [SerializeField] private Optional<float> _pitch;

        public Optional<AudioClip> AudioClip
        {
            get => _audioClip;
            set => _audioClip = value;
        }

        public Optional<AudioMixerGroup> AudioMixerGroup
        {
            get => _audioMixerGroup;
            set => _audioMixerGroup = value;
        }

        public Optional<float> Volume
        {
            get => _volume;
            set => _volume = value;
        }

        public Optional<float> SpatialBlend
        {
            get => _spatialBlend;
            set => _spatialBlend = value;
        }

        public Optional<Vector3> Position
        {
            get
            {
                if (_positionTransform.HasValue)
                    return _positionTransform.Value.position;

                return _position;
            }

            set
            {
                _positionTransform = Optional<Transform>.None();

                _position = value;
            }
        }

        public Optional<bool> Loop
        {
            get => _loop;
            set => _loop = value;
        }

        public Optional<float> Pitch
        {
            get => _pitch;
            set => _pitch = value;
        }
    }
}