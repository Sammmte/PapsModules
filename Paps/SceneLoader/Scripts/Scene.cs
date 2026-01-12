using SaintsField;
using SaintsField.Playa;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using PathClass = System.IO.Path;

namespace Paps.SceneLoading
{
    [Serializable]
    public struct Scene : IEquatable<Scene>, ISerializationCallbackReceiver
    {
        [SerializeField] private SceneReference _sceneReference;
        [field: ReadOnly] [ShowInInspector] public string Name { get; private set; }
        [field: ReadOnly] [ShowInInspector] public string Path { get; private set; }
        [field: ReadOnly] [ShowInInspector] public int BuildIndex { get; private set; }

        public Scene(string path, int buildIndex)
        {
            Path = path;
            Name = PathClass.GetFileNameWithoutExtension(Path);
            BuildIndex = buildIndex;

            _sceneReference = default;
        }

        public Scene(string name)
        {
            Name = name;
            Path = null;
            BuildIndex = -1;

            _sceneReference = default;
        }

        public void GetRootGameObjects(List<GameObject> list)
        {
            var unityScene = SceneManager.GetSceneByName(Name);

            unityScene.GetRootGameObjects(list);
        }

        public static implicit operator Scene(UnityEngine.SceneManagement.Scene scene)
        {
            if(scene.buildIndex < 0)
            {
                return new Scene(scene.name);
            }

            return new Scene(scene.path, scene.buildIndex);
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
