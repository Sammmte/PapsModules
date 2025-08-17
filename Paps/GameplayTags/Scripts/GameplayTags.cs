using System;
using System.Collections.Generic;
using UnityEngine;

namespace Paps.GameplayTags
{
    public abstract class GameplayTags<T> : GameplayTagsBase where T : struct, Enum
    {
        [SerializeField] private List<T> _tags;

        public void Add(T tag)
        {
            if(Contains(tag))
                return;
            
            _tags.Add(tag);
        }

        public void Add(ReadOnlySpan<T> tags)
        {
            for(int i = 0; i < tags.Length; i++)
                Add(tags[i]);
        }
        
        public bool Contains(T tag) => _tags.Contains(tag);

        public bool Contains(in ReadOnlySpan<T> tags)
        {
            for (int i = 0; i < tags.Length; i++)
            {
                if (!Contains(tags[i]))
                    return false;
            }

            return true;
        }

        public bool ContainsAny(in ReadOnlySpan<T> tags)
        {
            for (int i = 0; i < tags.Length; i++)
            {
                if (Contains(tags[i]))
                    return true;
            }

            return false;
        }
        
        public void Remove(T tag) => _tags.Remove(tag);

        public void Remove(in ReadOnlySpan<T> tags)
        {
            for(int i = 0; i < tags.Length; i++)
                Remove(tags[i]);
        }
    }
}