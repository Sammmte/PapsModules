using System;
using System.Collections.Generic;
using UnityEngine;

namespace Paps.Update
{
    public abstract class UpdateSchema<T> : ScriptableObject, IDisposable where T : IUpdateMethodListener
    {
        [SerializeField] private int _updatableGroupCapacity;
        [SerializeField] private List<string> _groups;
        [SerializeField] private List<FrameGroupsSequence> _frameSequence;

        private Dictionary<int, UpdateList<T>> _updatableGroups;

        [NonSerialized] private int _currentFrameIndex;

        public void Initialize()
        {
            _updatableGroups = new Dictionary<int, UpdateList<T>>(_groups.Count);

            _updatableGroups[UpdateSchemaUtils.DEFAULT_GROUP.GetId()] = new UpdateList<T>(_updatableGroupCapacity);

            for(int i = 0; i < _groups.Count; i++)
            {
                _updatableGroups[UpdateSchemaUtils.GetIdOfGroup(_groups[i])] = new UpdateList<T>(_updatableGroupCapacity);
            }

            _currentFrameIndex = 0;

            CreateDefaultSequenceIfEmpty();
        }

        private void CreateDefaultSequenceIfEmpty()
        {
            if(_frameSequence.Count == 0)
            {
                _frameSequence.Add(new FrameGroupsSequence() { GroupsSequence = new List<int>() { UpdateSchemaUtils.DEFAULT_GROUP.GetId() } });
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

        public void Register(T listener)
        {
            Register(listener, UpdateSchemaUtils.DEFAULT_GROUP.GetId());
        }

        public void Register(T listener, int updatableGroupId)
        {
            if(_updatableGroups.TryGetValue(updatableGroupId, out var updatables))
            {
                updatables.Add(listener);
                return;
            }

            throw new InvalidOperationException($"There is no group with id {updatableGroupId} in schema {name}");
        }

        public void Unregister(T listener)
        {
            Unregister(listener, UpdateSchemaUtils.DEFAULT_GROUP.GetId());
        }

        public void Unregister(T listener, int updatableGroupId)
        {
            if(_updatableGroups.TryGetValue(updatableGroupId, out var updatables))
            {
                updatables.Remove(listener);
                return;
            }

            throw new InvalidOperationException($"There is no group with id {updatableGroupId} in schema {name}");
        }

        public void Update()
        {
            var frameUpdateGroupIds = _frameSequence[_currentFrameIndex].GroupsSequence;

            if(frameUpdateGroupIds.Count > 0)
            {
                ExecuteUpdatesFor(frameUpdateGroupIds, _updatableGroups);
            }

            if(_currentFrameIndex == _frameSequence.Count - 1)
                _currentFrameIndex = 0;
            else
                _currentFrameIndex++;
        }

        protected abstract void ExecuteUpdatesFor(IReadOnlyList<int> groupIds, IReadOnlyDictionary<int, UpdateList<T>> updatableGroups);
    }
}