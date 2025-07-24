using System;
using System.Collections.Generic;
using UnityEngine;

namespace Paps.GameplayTags
{
    public static class GameplayTagsManager
    {
        private static List<GameplayTagsBase> _gameplayTags = new List<GameplayTagsBase>(1000);

        internal static void Register(GameplayTagsBase gameplayTags)
        {
            if(_gameplayTags.Contains(gameplayTags))
                return;
            
            _gameplayTags.Add(gameplayTags);
        }

        internal static void Unregister(GameplayTagsBase gameplayTags)
        {
            _gameplayTags.Remove(gameplayTags);
        }

        public static bool HasTag<TTag>(this GameObject gameObject, TTag tag) where TTag : struct, Enum
        {
            for (int i = 0; i < _gameplayTags.Count; i++)
            {
                var current = _gameplayTags[i];

                if (current.IsGameObjectTagged(gameObject) && 
                    current is GameplayTags<TTag> tagsComponent &&
                    HasTag(tagsComponent, tag))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool HasTag<TTag>(this Component component, TTag tag) where TTag : struct, Enum =>
            HasTag(component.gameObject, tag);
        
        public static bool HasTags<TTag>(this GameObject gameObject, ReadOnlySpan<TTag> tags) where TTag : struct, Enum
        {
            for (int i = 0; i < _gameplayTags.Count; i++)
            {
                var current = _gameplayTags[i];

                if (current.IsGameObjectTagged(gameObject) && 
                    current is GameplayTags<TTag> tagsComponent &&
                    HasTags(tagsComponent, tags))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool HasTags<TTag>(this Component component, ReadOnlySpan<TTag> tags) where TTag : struct, Enum =>
            HasTags(component.gameObject, tags);

        private static bool HasTag<TTag>(GameplayTags<TTag> gameplayTags, TTag tag)
            where TTag : struct, Enum
        {
            return gameplayTags.Tags.Contains(tag);
        }

        private static bool HasTags<TTag>(GameplayTags<TTag> gameplayTags, ReadOnlySpan<TTag> tags)
            where TTag : struct, Enum
        {
            for (int i = 0; i < tags.Length; i++)
            {
                if (!gameplayTags.Tags.Contains(tags[i]))
                    return false;
            }

            return true;
        }
    }
}