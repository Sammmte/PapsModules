using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Paps.Favorites
{
    [InitializeOnLoad]
    public static class FavoritesManager
    {
        public const string PREFS_SCOPE = "paps-favorites";
        public const string DEFAULT_GROUP_ID = "default";

        internal static FavoritesRepository Repository { get; private set; }
        private static Dictionary<string, FavoriteObjectGroup> _groups;

        public static Action OnDataChanged;

        static FavoritesManager()
        {
            Repository = new FavoritesRepository();

            _groups = LoadGroups();
        }

        private static Dictionary<string, FavoriteObjectGroup> LoadGroups()
        {
            var loadedGroups = Repository.LoadGroups();

            if(!loadedGroups.ContainsKey(DEFAULT_GROUP_ID))
            {
                loadedGroups[DEFAULT_GROUP_ID] = new FavoriteObjectGroup(DEFAULT_GROUP_ID);
            }

            return loadedGroups;
        }

        public static List<string> GetGroups() => _groups.Keys.ToList();

        public static List<FavoriteObject> GetFavoritesOf(string group) => _groups[group].OrderedFavorites.ToList();

        public static bool TryAddGroup(string group)
        {
            if(_groups.ContainsKey(group))
            {
                return false;
            }

            _groups[group] = new FavoriteObjectGroup(group);

            SaveAndNotify();

            return true;
        }

        public static void DeleteGroup(string group)
        {
            if(group == DEFAULT_GROUP_ID)
                return;

            _groups.Remove(group);

            SaveAndNotify();
        }

        public static FavoriteObject GetFavorite(string group, int index) => _groups[group].OrderedFavorites[index];

        public static void BatchResolve(FavoriteObject[] favorites)
        {
            UnityObject[] objects = new UnityObject[favorites.Length];
            GlobalObjectId.GlobalObjectIdentifiersToObjectsSlow(favorites.Select(f => f.Id).ToArray(), objects);

            for(int i = 0; i < favorites.Length; i++)
            {
                favorites[i].SetResolvedObject(objects[i]);
            }
        }

        public static void ResolveAll() => BatchResolve(_groups.SelectMany(g => g.Value.OrderedFavorites).ToArray());

        public static void ResolveAllUnresolved() => BatchResolve(_groups.SelectMany(g => g.Value.OrderedFavorites)
            .Where(f => f.Object == null)
            .ToArray()
            );

        public static void SetOrAddFavoritesAt(string targetGroup, List<FavoriteObject> favorites, int insertIndex)
        {
            var targetGroupObject = _groups[targetGroup];

            targetGroupObject.OrderedFavorites.RemoveAll(f => favorites.Contains(f));

            var finalIndex = Mathf.Clamp(insertIndex, 0, targetGroupObject.OrderedFavorites.Count);

            targetGroupObject.OrderedFavorites.InsertRange(finalIndex, favorites);

            SaveAndNotify();
        }

        public static void RemoveFavorites(string targetGroup, List<FavoriteObject> favorites)
        {
            var targetGroupObject = _groups[targetGroup];

            targetGroupObject.OrderedFavorites.RemoveAll(f => favorites.Contains(f));

            SaveAndNotify();
        }

        public static void Move(string sourceGroup, string targetGroup, List<FavoriteObject> favorites)
        {
            var sourceGroupObject = _groups[sourceGroup];

            sourceGroupObject.OrderedFavorites.RemoveAll(f => favorites.Contains(f));

            var targetGroupObject = _groups[targetGroup];

            var index = targetGroupObject.OrderedFavorites.Count - 1;

            SetOrAddFavoritesAt(targetGroup, favorites, index);
        }

        private static void SaveAndNotify()
        {
            Repository.Save(_groups);
            OnDataChanged?.Invoke();
        }
    }
}
