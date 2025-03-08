using Eflatun.SceneReference;
using System;
using UnityEngine;
using Paps.UnityExtensions;

namespace Paps.SceneLoading
{
    [Serializable]
    public struct SerializableSceneGroup
    {
        [SerializeField] private SceneReference[] _scenes;

        public static implicit operator SceneGroup(SerializableSceneGroup serializableSceneGroup)
        {
            if (serializableSceneGroup._scenes == null)
                return new SceneGroup(null);

            return new SceneGroup(serializableSceneGroup._scenes.ToArray<SceneReference, Scene>(s => s));
        }
    }
}
