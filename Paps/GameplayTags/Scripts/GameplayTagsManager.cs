using Paps.LevelSetup;
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
            var gameplayTags = gameObject.GetTags<TTag>();

            if (gameplayTags != null)
                return gameplayTags.Contains(tag);

            return false;
        }

        public static bool HasTag<TTag>(this Component component, TTag tag) where TTag : struct, Enum =>
            HasTag(component.gameObject, tag);
        
        public static bool HasTags<TTag>(this GameObject gameObject, ReadOnlySpan<TTag> tags) where TTag : struct, Enum
        {
            var gameplayTags = gameObject.GetTags<TTag>();

            if (gameplayTags != null)
                return gameplayTags.Contains(tags);

            return false;
        }

        public static bool HasTags<TTag>(this Component component, ReadOnlySpan<TTag> tags) where TTag : struct, Enum =>
            HasTags(component.gameObject, tags);

        public static void AddTag<TTag, TGameplayTagsComponent>(this Component component, TTag tag) 
            where TTag : struct, Enum where TGameplayTagsComponent : GameplayTags<TTag> =>
            AddTag<TTag, TGameplayTagsComponent>(component.gameObject, tag);

        public static void AddTag<TTag, TGameplayTagsComponent>(this GameObject gameObject, TTag tag)
            where TTag : struct, Enum where TGameplayTagsComponent : GameplayTags<TTag>
        {
            var gameplayTags = GetOrCreateTags<TTag, TGameplayTagsComponent>(gameObject);
            
            gameplayTags.Add(tag);
        }

        public static void AddTags<TTag, TGameplayTagsComponent>(this Component component, ReadOnlySpan<TTag> tags)
            where TTag : struct, Enum where TGameplayTagsComponent : GameplayTags<TTag> =>
            AddTags<TTag, TGameplayTagsComponent>(component.gameObject, tags);
        
        public static void AddTags<TTag, TGameplayTagsComponent>(this GameObject gameObject, ReadOnlySpan<TTag> tags)
            where TTag : struct, Enum where TGameplayTagsComponent : GameplayTags<TTag>
        {
            var gameplayTags = GetOrCreateTags<TTag, TGameplayTagsComponent>(gameObject);
            
            gameplayTags.Add(tags);
        }

        public static void RemoveTag<TTag>(this Component component, TTag tag) where TTag : struct, Enum =>
            RemoveTag(component.gameObject, tag);

        public static void RemoveTag<TTag>(this GameObject gameObject, TTag tag)
            where TTag : struct, Enum
        {
            var gameplayTags = GetTags<TTag>(gameObject);
            
            gameplayTags?.Remove(tag);
        }

        public static void RemoveTags<TTag>(this Component component, ReadOnlySpan<TTag> tags)
            where TTag : struct, Enum =>
            RemoveTags(component.gameObject, tags);

        public static void RemoveTags<TTag>(this GameObject gameObject, ReadOnlySpan<TTag> tags)
            where TTag : struct, Enum
        {
            var gameplayTags = GetTags<TTag>(gameObject);
            
            gameplayTags?.Remove(tags);
        }

        private static GameplayTags<TTag> GetTags<TTag>(this GameObject gameObject) where TTag : struct, Enum
        {
            for (int i = 0; i < _gameplayTags.Count; i++)
            {
                var current = _gameplayTags[i];

                if (current.IsGameObjectTagged(gameObject) && current is GameplayTags<TTag> gameplayTags)
                    return gameplayTags;
            }

            return null;
        }

        private static GameplayTags<TTag> GetOrCreateTags<TTag, TGameplayTagsComponent>(this GameObject gameObject) 
            where TTag : struct, Enum where TGameplayTagsComponent : GameplayTags<TTag>
        {
            var tags = GetTags<TTag>(gameObject);

            if (tags == null)
            {
                var newTags = gameObject.AddSetuppableComponent<TGameplayTagsComponent>();
                Register(newTags);
            }

            return tags;
        }
    }
}