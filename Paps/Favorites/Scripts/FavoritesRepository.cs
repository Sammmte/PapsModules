using Paps.UnityPrefs;
using System.Collections.Generic;
using System.Linq;
using Unity.Properties;
using UnityEditor;

[assembly: GeneratePropertyBagsForAssembly]

namespace Paps.Favorites
{
    internal class FavoritesRepository
    {
        [GeneratePropertyBag]
        internal struct FavoriteObjectGroupDTO
        {
            [CreateProperty] public string Id { get; set; }
            [CreateProperty] public List<FavoriteObjectDTO> OrderedFavorites;
        }

        [GeneratePropertyBag]
        internal struct FavoriteObjectDTO
        {
            [CreateProperty] public GlobalObjectId Id;
            [CreateProperty] public string LastKnownNameWithExtension;
        }

        private const string GROUPS_SAVE_KEY = "groups";

        private UnityPref _pref;

        public FavoritesRepository()
        {
            _pref = UnityPrefs.UnityPrefs.GetPref(UnityPrefType.UserProjectPrefs, FavoritesManager.PREFS_SCOPE);
        }

        public Dictionary<string, FavoriteObjectGroup> LoadGroups()
        {
            var dto = _pref.Get(GROUPS_SAVE_KEY, new Dictionary<string, FavoriteObjectGroupDTO>());

            return ConvertFromDTO(dto);
        }

        public void Save(Dictionary<string, FavoriteObjectGroup> groups)
        {
            _pref.Set(GROUPS_SAVE_KEY, ConvertToDTO(groups));
        }

        private Dictionary<string, FavoriteObjectGroupDTO> ConvertToDTO(Dictionary<string, FavoriteObjectGroup> groups)
        {
            return groups.ToDictionary(g => g.Key, g => new FavoriteObjectGroupDTO()
            {
                Id = g.Key,
                OrderedFavorites = ConvertToDTO(g.Value.OrderedFavorites)
            });
        }

        private List<FavoriteObjectDTO> ConvertToDTO(List<FavoriteObject> favorites)
        {
            return favorites.Select(f => new FavoriteObjectDTO() { Id = f.Id }).ToList();
        }

        private Dictionary<string, FavoriteObjectGroup> ConvertFromDTO(Dictionary<string, FavoriteObjectGroupDTO> dto)
        {
            return dto.ToDictionary(g => g.Key, g => new FavoriteObjectGroup(g.Key) { OrderedFavorites = ConvertFromDTO(g.Value.OrderedFavorites)});
        }

        private List<FavoriteObject> ConvertFromDTO(List<FavoriteObjectDTO> dto)
        {
            return dto.Select(f => new FavoriteObject(f.Id, f.LastKnownNameWithExtension)).ToList();
        }
    }
}
