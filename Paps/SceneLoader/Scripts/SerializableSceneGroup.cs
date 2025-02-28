using Eflatun.SceneReference;
using System;
using System.Linq;
using UnityEngine;

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

            return new SceneGroup(serializableSceneGroup._scenes.Select<SceneReference, Scene>(s => s).ToArray());
        }
    }
}
