﻿using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;

namespace Paps.Cheats
{
    public static class CheatsHelper
    {
        public static async UniTask<T> LoadAssetAsync<T>(this ICheatSubmenu cheatSubmenu, string address) where T : UnityEngine.Object
        {
            return await LoadAssetAsync<T>(address);
        }

        public static async UniTask<T> LoadAssetAsync<T>(this VisualElement visualElement, string address) where T : UnityEngine.Object
        {
            return await LoadAssetAsync<T>(address);
        }

        public static async UniTask<T> LoadAssetAsync<T>(string address) where T : UnityEngine.Object
        {
            return await Addressables.LoadAssetAsync<T>(address);
        }
    }
}
