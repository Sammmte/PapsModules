using Paps.ValueReferences;
using SaintsField;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Paps.Update
{
    public partial class UpdateManager : MonoBehaviour
    {
        [Serializable]
        private struct UpdaterConfiguration<T> where T : IUpdateMethodListener
        {
            [SerializeField] public ValueReference<int> Id;
            [SerializeField] private SaintsInterface<IUpdater<T>> _updater;
            public IUpdater<T> Updater => _updater.I;
        }

        public static UpdateManager Instance { get; private set; }
        
        [SerializeField] private UpdaterConfiguration<IUpdatable>[] _updateUpdaters;
        [SerializeField] private UpdaterConfiguration<ILateUpdatable>[] _lateUpdateUpdaters;
        [SerializeField] private UpdaterConfiguration<IFixedUpdatable>[] _fixedUpdateUpdaters;

        private Dictionary<int, IUpdater<IUpdatable>> _updateUpdatersDictionary;
        private Dictionary<int, IUpdater<ILateUpdatable>> _lateUpdateUpdatersDictionary;
        private Dictionary<int, IUpdater<IFixedUpdatable>> _fixedUpdateUpdatersDictionary;

        private bool _aboutToQuit;

        private void Awake()
        {
            Instance = this;
            
            DontDestroyOnLoad(gameObject);
            
            _aboutToQuit = false;
            
            _updateUpdatersDictionary = new Dictionary<int, IUpdater<IUpdatable>>(_updateUpdaters.Length);
            _lateUpdateUpdatersDictionary = new Dictionary<int, IUpdater<ILateUpdatable>>(_lateUpdateUpdaters.Length);
            _fixedUpdateUpdatersDictionary = new Dictionary<int, IUpdater<IFixedUpdatable>>(_fixedUpdateUpdaters.Length);
            
            for(int i = 0; i < _updateUpdaters.Length; i++)
            {
                var current = _updateUpdaters[i];
                _updateUpdatersDictionary[current.Id] = current.Updater;
            }
            
            for(int i = 0; i < _lateUpdateUpdaters.Length; i++)
            {
                var current = _lateUpdateUpdaters[i];
                _lateUpdateUpdatersDictionary[current.Id] = current.Updater;
            }
            
            for(int i = 0; i < _fixedUpdateUpdaters.Length; i++)
            {
                var current = _fixedUpdateUpdaters[i];
                _fixedUpdateUpdatersDictionary[current.Id] = current.Updater;
            }
        }
        
        private void Register<TUpdater, TListener>(TUpdater updater, TListener listener)
            where TListener : IUpdateMethodListener 
            where TUpdater : IUpdater<TListener>
        {
            if(_aboutToQuit)
                return;
            
            updater.Register(listener);
        }

        private void Register<TUpdater, TListener>(TUpdater updater, TListener listener, int updateSchemaGroupId)
            where TListener : IUpdateMethodListener 
            where TUpdater : IUpdater<TListener>
        {
            if(_aboutToQuit)
                return;
            
            updater.Register(listener, updateSchemaGroupId);
        }
        
        private void Unregister<TUpdater, TListener>(TUpdater updater, TListener listener)
            where TListener : IUpdateMethodListener 
            where TUpdater : IUpdater<TListener>
        {
            if(_aboutToQuit)
                return;
            
            updater.Unregister(listener);
        }

        private void Unregister<TUpdater, TListener>(TUpdater updater, TListener listener, int updateSchemaGroupId)
            where TListener : IUpdateMethodListener 
            where TUpdater : IUpdater<TListener>
        {
            if(_aboutToQuit)
                return;
            
            updater.Unregister(listener, updateSchemaGroupId);
        }

        private void OnApplicationQuit()
        {
            _aboutToQuit = true;

            DisposeAll();
        }

        private void DisposeAll()
        {
            for(int i = 0; i < _updateUpdaters.Length; i++)
            {
                _updateUpdaters[i].Updater.Dispose();
            }
            
            for(int i = 0; i < _lateUpdateUpdaters.Length; i++)
            {
                _lateUpdateUpdaters[i].Updater.Dispose();
            }
            
            for(int i = 0; i < _fixedUpdateUpdaters.Length; i++)
            {
                _fixedUpdateUpdaters[i].Updater.Dispose();
            }
        }
    }
}
