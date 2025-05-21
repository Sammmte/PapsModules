using Paps.UnityExtensions;
using System.Linq;
using UnityEngine;

namespace Paps.Physics
{
    public class NamedLayerMaskConfiguration : ScriptableObject
    {
        private static NamedLayerMaskConfiguration _instance;
        
        public static NamedLayerMaskConfiguration Instance
        {
            get
            {
#if UNITY_EDITOR
                if (_instance == null)
                {
                    _instance = GetInstanceOnEditor();
                }
#endif
                return _instance;
            }
        }
        
#if UNITY_EDITOR
        private static NamedLayerMaskConfiguration GetInstanceOnEditor()
        {
            var guids = UnityEditor.AssetDatabase.FindAssets($"t:{nameof(NamedLayerMaskConfiguration)}");

            if (guids == null || guids.Length == 0)
            {
                var newAsset = CreateInstance<NamedLayerMaskConfiguration>();
                UnityEditor.AssetDatabase.CreateAsset(newAsset, "Assets/NamedLayerMasks.asset");
                UnityEditor.AssetDatabase.SaveAssets();

                return newAsset;
            }
            
            var guid = guids.First();
            var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<NamedLayerMaskConfiguration>(assetPath);

            return asset;
        }
#endif
        
        [SerializeField] private NamedLayerMaskDefinition[] _namedLayerMasksDefinition;

        public NamedLayerMask[] GetAll() => _namedLayerMasksDefinition?.ToArray<NamedLayerMaskDefinition, NamedLayerMask>(definition => definition);

        private void Awake()
        {
            if (_instance == null)
                _instance = this;
        }

        public NamedLayerMask GetByName(string name)
        {
            return GetDefinitionByName(name);
        }

        internal NamedLayerMaskDefinition GetDefinitionByName(string name)
        {
            for (int i = 0; i < _namedLayerMasksDefinition.Length; i++)
            {
                if (_namedLayerMasksDefinition[i].Name == name)
                    return _namedLayerMasksDefinition[i];
            }

            return default;
        }
    }
}