using SaintsField;
using System;
using System.Linq;
using UnityEngine;

namespace Paps.Physics
{
    [Serializable]
    public struct NamedLayerMask
    {
        [SerializeField] [Dropdown(nameof(GetNamedLayerMasks))] private string _name;

        public string Name
        {
            get => _name;
            internal set => _name = value;
        }
        public LayerMask LayerMask => NamedLayerMaskConfiguration.Instance.GetDefinitionByName(_name).LayerMask;

        public static implicit operator LayerMask(NamedLayerMask namedLayerMask)
        {
            return namedLayerMask.LayerMask;
        }

        private DropdownList<string> GetNamedLayerMasks()
        {
#if UNITY_EDITOR
            return GetNamedLayerMasksAtEditTime();
#else
            return GetNamedLayerMasksAtRuntime();
#endif
        }

        private DropdownList<string> GetNamedLayerMasksAtRuntime() => new DropdownList<string>();

#if UNITY_EDITOR
        private DropdownList<string> GetNamedLayerMasksAtEditTime()
        {
            var namedLayerMasks = NamedLayerMaskConfiguration.Instance.GetAll();

            if (namedLayerMasks == null)
                return new DropdownList<string>();
            
            return new DropdownList<string>(namedLayerMasks.Select(namedLayerMask => (namedLayerMask.Name, namedLayerMask.Name)));
        }
#endif
    }
}