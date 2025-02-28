using Paps.ProjectSetup;
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

namespace Paps.Audio.ProjectSetup
{
    public class AudioProjectSetupSettingsHandler : IProjectSetupSettingsHandler
    {
        private const string BASE_PATH = "Assets/Game/Audio";
        private static readonly string SETUP_ASSET_PATH = Path.Combine(BASE_PATH, "AudioGameSetupProcess.asset");
        private static readonly string AUDIO_MIXER_ASSET_PATH = Path.Combine(BASE_PATH, "AudioMixer.mixer");

        public Type SettingsType => typeof(AudioProjectSetupSettings);

        public void HandleSettings(object customSettings)
        {
            Directory.CreateDirectory(BASE_PATH);
            var defaultAudioMixerPath = AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("DefaultAudioMixer").First());

            AssetDatabase.CopyAsset(defaultAudioMixerPath, AUDIO_MIXER_ASSET_PATH);

            var newAudioMixer = AssetDatabase.LoadAssetAtPath<AudioMixer>(AUDIO_MIXER_ASSET_PATH);
            var newSetupAsset = ScriptableObject.CreateInstance<AudioGameSetupProcess>();
            var setupAssetSerializedObject = new SerializedObject(newSetupAsset);

            setupAssetSerializedObject.FindProperty("_audioMixer").objectReferenceValue = newAudioMixer;
            setupAssetSerializedObject.ApplyModifiedProperties();

            AssetDatabase.CreateAsset(newSetupAsset, SETUP_ASSET_PATH);
        }
    }
}
