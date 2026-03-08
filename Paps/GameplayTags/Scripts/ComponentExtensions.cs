using Paps.Levels;
using Paps.UnityExtensions;
using System;
using UnityEngine;

namespace Paps.GameplayTags
{
    public static class ComponentExtensions
    {
        public static bool HasTag<TTag>(this GameObject gameObject, TTag tag) where TTag : struct, Enum
        {
            var gameplayTags = gameObject.GetTags<TTag>();

            if (gameplayTags != null)
                return gameplayTags.Contains(tag);

            return false;
        }

        public static bool HasTag<TComponent, TTag>(this TComponent extended, TTag tag) where TComponent : class where TTag : struct, Enum
        {
            return HasTag(extended.AsUnityComponent().gameObject, tag);
        }
        
        public static bool HasTags<TTag>(this GameObject gameObject, ref ReadOnlySpan<TTag> tags) where TTag : struct, Enum
        {
            var gameplayTags = gameObject.GetTags<TTag>();

            if (gameplayTags != null)
                return gameplayTags.Contains(tags);

            return false;
        }

        public static bool HasTags<TComponent, TTag>(this TComponent component, ref ReadOnlySpan<TTag> tags) where TComponent : class where TTag : struct, Enum
        {
            return HasTags(component.AsUnityComponent().gameObject, ref tags);
        }
            
        public static bool HasAnyTag<TTag>(this GameObject gameObject, ref ReadOnlySpan<TTag> tags)
            where TTag : struct, Enum
        {
            var gameplayTags = gameObject.GetTags<TTag>();

            if (gameplayTags != null)
                return gameplayTags.ContainsAny(tags);

            return false;
        }

        public static bool HasAnyTag<TComponent, TTag>(this TComponent component, ref ReadOnlySpan<TTag> tags)
            where TComponent : class where TTag : struct, Enum =>
            HasAnyTag(component.AsUnityComponent().gameObject, ref tags);

        public static void AddTag<TComponent, TTag, TGameplayTagsComponent>(this TComponent component, TTag tag) 
            where TComponent : class where TTag : struct, Enum where TGameplayTagsComponent : GameplayTags<TTag> =>
            AddTag<TTag, TGameplayTagsComponent>(component.AsUnityComponent().gameObject, tag);

        public static void AddTag<TTag, TGameplayTagsComponent>(this GameObject gameObject, TTag tag)
            where TTag : struct, Enum where TGameplayTagsComponent : GameplayTags<TTag>
        {
            var gameplayTags = GetOrCreateTags<TTag, TGameplayTagsComponent>(gameObject);
            
            gameplayTags.Add(tag);
        }

        public static void AddTags<TComponent, TTag, TGameplayTagsComponent>(this TComponent component, ref ReadOnlySpan<TTag> tags)
            where TComponent : class where TTag : struct, Enum where TGameplayTagsComponent : GameplayTags<TTag> =>
            AddTags<TTag, TGameplayTagsComponent>(component.AsUnityComponent().gameObject, ref tags);
        
        public static void AddTags<TTag, TGameplayTagsComponent>(this GameObject gameObject, ref ReadOnlySpan<TTag> tags)
            where TTag : struct, Enum where TGameplayTagsComponent : GameplayTags<TTag>
        {
            var gameplayTags = GetOrCreateTags<TTag, TGameplayTagsComponent>(gameObject);
            
            gameplayTags.Add(tags);
        }

        public static void RemoveTag<TComponent, TTag>(this TComponent component, TTag tag) where TComponent : class where TTag : struct, Enum =>
            RemoveTag(component.AsUnityComponent().gameObject, tag);

        public static void RemoveTag<TTag>(this GameObject gameObject, TTag tag)
            where TTag : struct, Enum
        {
            var gameplayTags = GetTags<TTag>(gameObject);
            
            gameplayTags?.Remove(tag);
        }

        public static void RemoveTags<TComponent, TTag>(this TComponent component, ref ReadOnlySpan<TTag> tags)
            where TComponent : class where TTag : struct, Enum =>
            RemoveTags(component.AsUnityComponent().gameObject, ref tags);

        public static void RemoveTags<TTag>(this GameObject gameObject, ref ReadOnlySpan<TTag> tags)
            where TTag : struct, Enum
        {
            var gameplayTags = GetTags<TTag>(gameObject);
            
            gameplayTags?.Remove(tags);
        }

        private static GameplayTags<TTag> GetTags<TTag>(this GameObject gameObject) where TTag : struct, Enum
        {
            return GameplayTagsManager.Instance.GetTags<TTag>(gameObject);
        }

        private static GameplayTags<TTag> GetOrCreateTags<TTag, TGameplayTagsComponent>(this GameObject gameObject) 
            where TTag : struct, Enum where TGameplayTagsComponent : GameplayTags<TTag>
        {
            var tags = GetTags<TTag>(gameObject);

            if (tags == null)
            {
                gameObject.AddLevelBoundComponent<TGameplayTagsComponent>();
            }

            return tags;
        }
    }
}