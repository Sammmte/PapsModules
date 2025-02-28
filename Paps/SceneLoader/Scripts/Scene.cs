using Eflatun.SceneReference;
using System;

namespace Paps.SceneLoading
{
    public readonly struct Scene : IEquatable<Scene>
    {
        public string Name { get; }
        public string Path { get; }

        public Scene(string name, string path)
        {
            Name = name;
            Path = path;
        }

        public static implicit operator Scene(UnityEngine.SceneManagement.Scene scene)
        {
            return new Scene(scene.name, scene.path);
        }

        public static implicit operator Scene(SceneReference sceneReference)
        {
            return new Scene(sceneReference.Name, sceneReference.Path);
        }

        public bool Equals(Scene other)
        {
            return other.Path == Path;
        }
    }
}
