using SaintsField.Playa;
using System.Collections.Generic;
using UnityEngine;

namespace Paps.ComponentOwnership
{
    internal class ComponentOwnershipManager : MonoBehaviour
    {
        internal static ComponentOwnershipManager Instance { get; private set; }
        
        [SerializeField] private int _ownersCapacity;
        [SerializeField] private int _ownershipRelationsCapacity;
        [ShowInInspector] private List<ComponentOwner> _owners;

        private Dictionary<Component, ComponentOwner> _ownershipDictionary;

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            _owners = new List<ComponentOwner>(_ownersCapacity);
            _ownershipDictionary = new Dictionary<Component, ComponentOwner>(_ownershipRelationsCapacity);
        }

        public void Register(ComponentOwner owner)
        {
            if(_owners.Contains(owner))
                return;
            
            _owners.Add(owner);
            CacheOwnedComponentsOf(owner);
        }

        public void Unregister(ComponentOwner owner)
        {
            _owners.Remove(owner);
            ClearOwnedComponentsOf(owner);
        }

        public bool TryGetOwner<T>(Component component, out T owner)
        {
            owner = default;
            
            if (_ownershipDictionary.TryGetValue(component, out ComponentOwner o) && o.IdentityComponent is T casted)
            {
                owner = casted;
                return true;
            }

            return false;
        }

        private void CacheOwnedComponentsOf(ComponentOwner owner)
        {
            for (int i = 0; i < owner.OwnedComponents.Count; i++)
            {
                _ownershipDictionary.Add(owner.OwnedComponents[i], owner);
            }
        }

        private void ClearOwnedComponentsOf(ComponentOwner owner)
        {
            for (int i = 0; i < owner.OwnedComponents.Count; i++)
            {
                _ownershipDictionary.Remove(owner.OwnedComponents[i]);
            }
        }
    }
}