using SaintsField.Playa;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Paps.GameplayTags
{
    public class GameplayTagsManager : MonoBehaviour
    {
        public static GameplayTagsManager Instance { get; private set; }
        
        [SerializeField] private int _gameObjectCapacity;

        [ShowInInspector] private Dictionary<GameObject, GameplayTagsBase> _taggedObjects;

        private void Awake()
        {
            _taggedObjects = new Dictionary<GameObject, GameplayTagsBase>(_gameObjectCapacity);

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        internal void Register(GameplayTagsBase gameplayTags)
        {
            _taggedObjects[gameplayTags.gameObject] = gameplayTags;
        }

        internal void Unregister(GameplayTagsBase gameplayTags)
        {
            _taggedObjects.Remove(gameplayTags.gameObject);
        }

        public GameplayTags<TTag> GetTags<TTag>(GameObject go) where TTag : struct, Enum
        {
            if (_taggedObjects.ContainsKey(go) && _taggedObjects[go] is GameplayTags<TTag> tags)
                return tags;

            return null;
        }
    }
}