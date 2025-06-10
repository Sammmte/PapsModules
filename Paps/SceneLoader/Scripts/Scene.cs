using Eflatun.SceneReference;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Paps.SceneLoading
{
    public readonly struct Scene : IEquatable<Scene>
    {
        public string Name { get; }
        public string Path { get; }
        public int BuildIndex { get; }

        public Scene(string name, string path, int buildIndex)
        {
            Name = name;
            Path = path;
            BuildIndex = buildIndex;
        }

        public void GetRootGameObjects(List<GameObject> list)
        {
            var unityScene = SceneManager.GetSceneByBuildIndex(BuildIndex);

            unityScene.GetRootGameObjects(list);
        }

        public static implicit operator Scene(UnityEngine.SceneManagement.Scene scene)
        {
            return new Scene(scene.name, scene.path, scene.buildIndex);
        }

        public static implicit operator Scene(SceneReference sceneReference)
        {
            return new Scene(sceneReference.Name, sceneReference.Path, sceneReference.BuildIndex);
        }

        public bool Equals(Scene other)
        {
            return other.BuildIndex == BuildIndex;
        }
    }
}
