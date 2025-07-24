using System;
using System.Collections.Generic;
using UnityEngine;

namespace Paps.GameplayTags
{
    public abstract class GameplayTags<T> : GameplayTagsBase where T : struct, Enum
    {
        [field: SerializeField] public List<T> Tags { get; private set; }
    }
}