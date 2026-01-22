using System;
using System.IO;
using Unity.Properties;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Paps.Favorites
{
    public class FavoriteObject : IEquatable<FavoriteObject>
    {
        private const int SCENE_OBJECT_IDENTIFIER_TYPE = 2;

        public bool IsSceneObject => Id.identifierType == SCENE_OBJECT_IDENTIFIER_TYPE;
        public GlobalObjectId Id { get; }
        public UnityObject Object { get; private set; }
        public string LastKnownNameWithExtension { get; private set; }

        public FavoriteObject(GlobalObjectId id)
        {
            Id = id;
        }

        internal FavoriteObject(GlobalObjectId id, string lastKnownNameWithExtension)
        {
            Id = id;
            LastKnownNameWithExtension = lastKnownNameWithExtension;
        }

        public void SetResolvedObject(UnityObject obj)
        {
            Object = obj;

            LastKnownNameWithExtension = Path.GetFileName(AssetDatabase.GetAssetPath(obj));
        }

        public bool Equals(FavoriteObject other)
        {
            return Id.Equals(other.Id);
        }
    }
}
