using UnityEditor;
using UnityEngine;

namespace Paps.Favorites
{
    public static class FavoriteTypeDisplayHelper
    {
        public static string GetTypeDisplayName(Object obj)
        {
            var assetPath = AssetDatabase.GetAssetPath(obj);

            var prefabType = PrefabUtility.GetPrefabAssetType(obj);

            if(AssetDatabase.IsValidFolder(assetPath))
            {
                return "Folder";
            }
            else
            {
                switch(prefabType)
                {
                    case PrefabAssetType.Regular: return "Prefab";
                    case PrefabAssetType.Variant: return "Prefab Variant";
                    case PrefabAssetType.Model: return "Model";
                }

                return obj.GetType().Name;
            }
        }
    }
}
