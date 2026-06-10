using System;
using System.Collections.Generic;
using UnityEngine;

namespace Paps.Update
{
    public abstract class UpdateSchema<T> : ScriptableObject, IDisposable where T : IUpdateMethodListener
    {
        [SerializeField] private int _updatableGroupCapacity;
        [SerializeField] private UpdateSchemaGroup _defaultGroup;
        [SerializeField] private List<UpdateSchemaGroup> _groups;
        [SerializeField] private List<FrameGroupsSequence> _frameSequence;
        
        [NonSerialized] private Dictionary<UpdateSchemaGroup, UpdateList<T>> _updatableGroups;

        [NonSerialized] private int _currentFrameIndex;

        public void Initialize()
        {
            _updatableGroups = new Dictionary<UpdateSchemaGroup, UpdateList<T>>(_groups.Count + 1);

            if (_defaultGroup != null)
                _updatableGroups[_defaultGroup] = new UpdateList<T>(_updatableGroupCapacity);

            for(int i = 0; i < _groups.Count; i++)
            {
                if (_groups[i] != null && !_updatableGroups.ContainsKey(_groups[i]))
                    _updatableGroups[_groups[i]] = new UpdateList<T>(_updatableGroupCapacity);
            }

            _currentFrameIndex = 0;

            CreateDefaultSequenceIfEmpty();
        }

        private void CreateDefaultSequenceIfEmpty()
        {
            if(_frameSequence.Count == 0 && _defaultGroup != null)
            {
                _frameSequence.Add(new FrameGroupsSequence() { GroupsSequence = new List<UpdateSchemaGroup>() { _defaultGroup } });
            }
        }

        public void Dispose()
        {
            _updatableGroups = null;
        }

        public bool HasListeners()
        {
            foreach(var keyValue in _updatableGroups)
            {
                if(keyValue.Value.Count > 0)
                    return true;
            }

            return false;
        }

        public void Register(T listener, UpdateSchemaGroup updatableGroup)
        {
            if (updatableGroup == null)
                updatableGroup = _defaultGroup;

            if(_updatableGroups.TryGetValue(updatableGroup, out var updatables))
            {
                updatables.Add(listener);
                return;
            }

            throw new InvalidOperationException($"There is no group {updatableGroup.name} in schema {name}");
        }

        public void Unregister(T listener, UpdateSchemaGroup updatableGroup)
        {
            if (updatableGroup == null)
                updatableGroup = _defaultGroup;

            if(_updatableGroups.TryGetValue(updatableGroup, out var updatables))
            {
                updatables.Remove(listener);
                return;
            }

            throw new InvalidOperationException($"There is no group {updatableGroup.name} in schema {name}");
        }

        public void Update()
        {
            if (_frameSequence.Count == 0) return;

            var frameUpdateGroups = _frameSequence[_currentFrameIndex].GroupsSequence;

            if(frameUpdateGroups != null && frameUpdateGroups.Count > 0)
            {
                ExecuteUpdatesFor(frameUpdateGroups, _updatableGroups);
            }

            if(_currentFrameIndex >= _frameSequence.Count - 1)
                _currentFrameIndex = 0;
            else
                _currentFrameIndex++;
        }

        protected abstract void ExecuteUpdatesFor(IReadOnlyList<UpdateSchemaGroup> groups, IReadOnlyDictionary<UpdateSchemaGroup, UpdateList<T>> updatableGroups);

#if UNITY_EDITOR
        private void OnValidate()
        {
            if(Application.isPlaying)
                return;

            if (_groups == null) return;

            for (int i = _groups.Count - 1; i >= 0; i--)
            {
                if (_groups[i] == _defaultGroup)
                {
                    Debug.LogWarning($"Removed default update schema group {_defaultGroup.name} from groups list");
                    _groups.RemoveAt(i);
                    continue;
                }
            }
        }
#endif
    }
}