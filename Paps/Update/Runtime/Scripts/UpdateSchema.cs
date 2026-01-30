using Paps.Logging;
using SaintsField.Playa;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Paps.Update
{
    public abstract class UpdateSchema<T> : ScriptableObject, IDisposable where T : IUpdateMethodListener
    {
        [SerializeField] private int _updatableGroupCapacity;
        [SerializeField] private List<UpdatableGroup> _groups;
        [SerializeField] private FrameGroupsSequence[] _frameSequence;

        [ShowInInspector] private Dictionary<int, UpdateList<T>> _updatableGroups;
        
        private int _groupCount;
        private List<int>[] _cookedFrameSequence;

        [NonSerialized] private int _currentFrameIndex;

        public void Initialize()
        {
            CookFrameSequence();

            _groupCount = _groups.Count + 1;

            _updatableGroups = new Dictionary<int, UpdateList<T>>(_groupCount);

            for(int i = 0; i < _groupCount; i++)
            {
                _updatableGroups[i] = new UpdateList<T>(_updatableGroupCapacity);
            }

            _currentFrameIndex = 0;
        }

        private void CookFrameSequence()
        {
            _cookedFrameSequence = new List<int>[_frameSequence.Length];

            for(int i = 0; i < _frameSequence.Length; i++)
            {
                var frameUpdateGroup = _frameSequence[i];
                var newList = new List<int>(frameUpdateGroup.GroupsSequence.Length);

                for(int j = 0; j < frameUpdateGroup.GroupsSequence.Length; j++)
                {
                    newList.Add(GetIdOfGroup(frameUpdateGroup.GroupsSequence[j]));
                }

                _cookedFrameSequence[i] = newList;
            }
        }

        private int GetIdOfGroup(string groupName)
        {
            if(groupName == UpdatableGroup.DEFAULT_GROUP_NAME)
                return UpdatableGroup.DEFAULT_GROUP_ID;

            for(int i = 0; i < _groups.Count; i++)
            {
                if(_groups[i].Name == groupName)
                    return i;
            }

            return -1;
        }

        public void Dispose()
        {
            _updatableGroups = null;
        }

        public bool HasListeners()
        {
            for(int i = 0; i < _groupCount; i++)
            {
                var updatables = _updatableGroups[i];

                if(updatables.Count > 0)
                    return true;
            }

            return false;
        }

        public void Register(T listener)
        {
            Register(listener, UpdatableGroup.DEFAULT_GROUP_ID);
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
            Unregister(listener, UpdatableGroup.DEFAULT_GROUP_ID);
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
            if(_cookedFrameSequence.Length == 0)
            {
                this.LogWarning($"Update schema {name} has no frame sequence!");
                return;
            }

            var frameUpdateGroupIds = _cookedFrameSequence[_currentFrameIndex];

            if(frameUpdateGroupIds.Count > 0)
            {
                ExecuteUpdatesFor(frameUpdateGroupIds, _updatableGroups);
            }

            if(_currentFrameIndex == _frameSequence.Length - 1)
                _currentFrameIndex = 0;
            else
                _currentFrameIndex++;
        }

        protected abstract void ExecuteUpdatesFor(IReadOnlyList<int> groupIds, IReadOnlyDictionary<int, UpdateList<T>> updatableGroups);
    }
}