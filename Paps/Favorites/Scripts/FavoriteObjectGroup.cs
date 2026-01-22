using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Paps.Favorites
{
    internal class FavoriteObjectGroup
    {
        public string Id;
        public List<FavoriteObject> OrderedFavorites;

        public FavoriteObjectGroup(string id)
        {
            Id = id;
            OrderedFavorites = new List<FavoriteObject>();
        }
    }
}
