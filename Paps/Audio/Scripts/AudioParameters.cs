using Paps.Optionals;
using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Paps.Audio
{
    [Serializable]
    public struct AudioParameters
    {
        [field: SerializeField] public Optional<AudioClip> AudioClip { get; private set; }
        [field: SerializeField] public Optional<AudioMixerGroup> AudioMixerGroup  { get; private set; }
        [field: SerializeField] public Optional<float> Volume { get; private set; }
        [field: SerializeField] public Optional<float> SpatialBlend { get; private set; }
        [SerializeField] private Optional<Transform> _positionTransform;
        [SerializeField] private Optional<Vector3> _position;
        [field: SerializeField] public Optional<bool> Loop { get; private set; }
        [field: SerializeField] public Optional<float> Pitch { get; private set; }

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
    }
}