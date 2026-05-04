using System;
using System.Collections.Generic;
using UnityEngine;

namespace Paps.Update
{
    public partial class UpdateManager : MonoBehaviour
    {
        public static UpdateManager Instance { get; private set; }
        
        [SerializeField] private List<Updater<IUpdatable>> _updateUpdaters;
        [SerializeField] private List<Updater<ILateUpdatable>> _lateUpdateUpdaters;
        [SerializeField] private List<Updater<IFixedUpdatable>> _fixedUpdateUpdaters;

        private bool _aboutToQuit;

        private void Awake()
        {
            Instance = this;
            
            DontDestroyOnLoad(gameObject);
            
            _aboutToQuit = false;

            InitializeUpdaters(_updateUpdaters);
            InitializeUpdaters(_lateUpdateUpdaters);
            InitializeUpdaters(_fixedUpdateUpdaters);
        }

        private void InitializeUpdaters<T>(List<Updater<T>> updaters) where T : IUpdateMethodListener
        {
            for(int i = 0; i < updaters.Count; i++)
            {
                updaters[i].Initialize();
            }
        }

        public bool ContainsUpdater<T>(Updater<T> updater) where T : IUpdateMethodListener
        {
            switch(updater)
            {
                case Updater<IUpdatable> updateUpdater:
                    return _updateUpdaters.Contains(updateUpdater);

                case Updater<ILateUpdatable> lateUpdateUpdater:
                    return _lateUpdateUpdaters.Contains(lateUpdateUpdater);

                case Updater<IFixedUpdatable> fixedUpdateUpdater:
                    return _fixedUpdateUpdaters.Contains(fixedUpdateUpdater);
            }

            return false;
        }
        
        private void ThrowUpdaterNotFoundException<T>(Updater<T> updater) where T : IUpdateMethodListener
        {
            throw new ArgumentException($"Updater {updater.name} not found on UpdateManager");
        }
        
        private void Register<TUpdater, TListener>(TUpdater updater, TListener listener)
            where TListener : IUpdateMethodListener 
            where TUpdater : Updater<TListener>
        {
            if(_aboutToQuit)
                return;
            
            updater.Register(listener);
        }

        private void Register<TUpdater, TListener>(TUpdater updater, TListener listener, int updateSchemaGroupId)
            where TListener : IUpdateMethodListener 
            where TUpdater : Updater<TListener>
        {
            if(_aboutToQuit)
                return;
            
            updater.Register(listener, updateSchemaGroupId);
        }
        
        private void Unregister<TUpdater, TListener>(TUpdater updater, TListener listener)
            where TListener : IUpdateMethodListener 
            where TUpdater : Updater<TListener>
        {
            if(_aboutToQuit)
                return;
            
            updater.Unregister(listener);
        }

        private void Unregister<TUpdater, TListener>(TUpdater updater, TListener listener, int updateSchemaGroupId)
            where TListener : IUpdateMethodListener 
            where TUpdater : Updater<TListener>
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
            for(int i = 0; i < _updateUpdaters.Count; i++)
            {
                _updateUpdaters[i].Dispose();
            }
            
            for(int i = 0; i < _lateUpdateUpdaters.Count; i++)
            {
                _lateUpdateUpdaters[i].Dispose();
            }
            
            for(int i = 0; i < _fixedUpdateUpdaters.Count; i++)
            {
                _fixedUpdateUpdaters[i].Dispose();
            }
        }
    }
}
