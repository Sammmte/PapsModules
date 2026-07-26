using SaintsField;
using SaintsField.Playa;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using PathClass = System.IO.Path;
using UnityScene = UnityEngine.SceneManagement.Scene;

namespace Paps.SceneLoading
{
    [Serializable]
    public struct Scene : IEquatable<Scene>, ISerializationCallbackReceiver
    {
        [SerializeField] private SceneReference _sceneReference;
        [NonSerialized] private UnityScene _unityScene;

        private UnityScene LoadedUnityScene
        {
            get
            {
                if(!_unityScene.IsValid())
                {
                    _unityScene = SceneManager.GetSceneByName(Name);
                }

                return _unityScene;
            }
        }

        [field: ReadOnly] [ShowInInspector] public string Name { get; private set; }
        [field: ReadOnly] [ShowInInspector] public string Path { get; private set; }
        [field: ReadOnly] [ShowInInspector] public int BuildIndex { get; private set; }

        public Scene(string path, int buildIndex)
        {
            Path = path;
            Name = PathClass.GetFileNameWithoutExtension(Path);
            BuildIndex = buildIndex;

            _sceneReference = default;
            _unityScene = default;
        }

        public Scene(string name)
        {
            Name = name;
            Path = null;
            BuildIndex = -1;

            _sceneReference = default;
            _unityScene = default;
        }

        private Scene(UnityScene unityScene)
        {
            Name = unityScene.name;
            Path = unityScene.path;
            BuildIndex = unityScene.buildIndex;

            _sceneReference = default;
            _unityScene = unityScene;
        }

        public void GetRootGameObjects(List<GameObject> list)
        {
            LoadedUnityScene.GetRootGameObjects(list);
        }

        public static implicit operator Scene(UnityScene unityScene)
        {
            return new Scene(unityScene);
        }

        public bool Equals(Scene other)
        {
            return other.Name == Name;
        }

        public void OnBeforeSerialize()
        {
            
        }

        public void OnAfterDeserialize()
        {
            Path = _sceneReference.path;
            Name = PathClass.GetFileNameWithoutExtension(Path);
            BuildIndex = _sceneReference.index;
        }

        #if UNITY_EDITOR
        public string GetSceneAssetPath() => $"Assets/{Path}.unity";
        #endif
    }
}
