using Cysharp.Threading.Tasks;
using Paps.GameSetup;
using Paps.GlobalProvisioning;
using UnityEngine;
using UnityEngine.Audio;

namespace Paps.Audio
{
    [CreateAssetMenu(menuName = "Paps/Setup/Audio")]
    public class AudioGameSetupProcess : GameSetupProcess
    {
        [SerializeField] private AudioMixer _audioMixer;
        [SerializeField] private AudioEmitter _audioEmitterPrefab;

        public override async UniTask Setup()
        {
            var audioService = new AudioService(_audioMixer, _audioEmitterPrefab);
            Locator.Create(audioService);
        }
    }
}
