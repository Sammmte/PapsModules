using SaintsField;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Paps.Update
{
    public class UpdateManager : MonoBehaviour
    {
        public static UpdateManager Instance { get; private set; }
        
        [SerializeField] private SaintsInterface<IUpdater<IUpdatable>>[] _updateUpdaters;
        [SerializeField] private SaintsInterface<IUpdater<ILateUpdatable>>[] _lateUpdateUpdaters;
        [SerializeField] private SaintsInterface<IUpdater<IFixedUpdatable>>[] _fixedUpdateUpdaters;

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
                var current = _updateUpdaters[i].I;
                _updateUpdatersDictionary[current.Id] = current;
            }
            
            for(int i = 0; i < _lateUpdateUpdaters.Length; i++)
            {
                var current = _lateUpdateUpdaters[i].I;
                _lateUpdateUpdatersDictionary[current.Id] = current;
            }
            
            for(int i = 0; i < _fixedUpdateUpdaters.Length; i++)
            {
                var current = _fixedUpdateUpdaters[i].I;
                _fixedUpdateUpdatersDictionary[current.Id] = current;
            }
        }
        
        private IUpdater<IUpdatable> GetDefaultUpdateUpdater()
        {
            return _updateUpdaters[0].I;
        }

        private IUpdater<ILateUpdatable> GetDefaultLateUpdateUpdater()
        {
            return _lateUpdateUpdaters[0].I;
        }
        
        private IUpdater<IFixedUpdatable> GetDefaultFixedUpdateUpdater()
        {
            return _fixedUpdateUpdaters[0].I;
        }
        
        public IUpdater<IUpdatable> GetUpdateUpdaterById(int id)
        {
            if(_updateUpdatersDictionary.TryGetValue(id, out var updater))
            {
                return updater;
            }
            else
            {
                throw new ArgumentException($"No Update Updater found with Id {id}");
            }
        }
        
        public IUpdater<ILateUpdatable> GetLateUpdateUpdaterById(int id)
        {
            if(_lateUpdateUpdatersDictionary.TryGetValue(id, out var updater))
            {
                return updater;
            }
            else
            {
                throw new ArgumentException($"No Late Update Updater found with Id {id}");
            }
        }
        
        public IUpdater<IFixedUpdatable> GetFixedUpdateUpdaterById(int id)
        {
            if(_fixedUpdateUpdatersDictionary.TryGetValue(id, out var updater))
            {
                return updater;
            }
            else
            {
                throw new ArgumentException($"No Fixed Update Updater found with Id {id}");
            }
        }

        public void RegisterForUpdate(IUpdatable updatable)
        {
            Register(GetDefaultUpdateUpdater(), updatable);
        }
        
        public void RegisterForUpdate(IUpdatable updatable, int updaterId)
        {
            if(_updateUpdatersDictionary.TryGetValue(updaterId, out var updater))
            {
                Register(updater, updatable);
            }
            else
            {
                throw new ArgumentException($"No Update Updater found with Id {updaterId}");
            }
        }
        
        public void UnregisterFromUpdate(IUpdatable updatable)
        {
            Unregister(GetDefaultUpdateUpdater(), updatable);
        }

        public void UnregisterFromUpdate(IUpdatable updatable, int updaterId)
        {
            if(_updateUpdatersDictionary.TryGetValue(updaterId, out var updater))
            {
                Unregister(updater, updatable);
            }
            else
            {
                throw new ArgumentException($"No Update Updater found with Id {updaterId}");
            }
        }
        
        public void RegisterForLateUpdate(ILateUpdatable lateUpdatable)
        {
            Register(GetDefaultLateUpdateUpdater(), lateUpdatable);
        }
        
        public void RegisterForLateUpdate(ILateUpdatable lateUpdatable, int updaterId)
        {
            if(_lateUpdateUpdatersDictionary.TryGetValue(updaterId, out var updater))
            {
                Register(updater, lateUpdatable);
            }
            else
            {
                throw new ArgumentException($"No Late Update Updater found with Id {updaterId}");
            }
        }
        
        public void UnregisterFromLateUpdate(ILateUpdatable lateUpdatable)
        {
            Unregister(GetDefaultLateUpdateUpdater(), lateUpdatable);
        }
        
        public void UnregisterFromLateUpdate(ILateUpdatable lateUpdatable, int updaterId)
        {
            if(_lateUpdateUpdatersDictionary.TryGetValue(updaterId, out var updater))
            {
                Unregister(updater, lateUpdatable);
            }
            else
            {
                throw new ArgumentException($"No Late Update Updater found with Id {updaterId}");
            }
        }
        
        public void RegisterForFixedUpdate(IFixedUpdatable fixedUpdatable)
        {
            Register(GetDefaultFixedUpdateUpdater(), fixedUpdatable);
        }
        
        public void RegisterForFixedUpdate(IFixedUpdatable fixedUpdatable, int updaterId)
        {
            if(_fixedUpdateUpdatersDictionary.TryGetValue(updaterId, out var updater))
            {
                Register(updater, fixedUpdatable);
            }
            else
            {
                throw new ArgumentException($"No Fixed Update Updater found with Id {updaterId}");
            }
        }
        
        public void UnregisterFromFixedUpdate(IFixedUpdatable fixedUpdatable)
        {
            Unregister(GetDefaultFixedUpdateUpdater(), fixedUpdatable);
        }
        
        public void UnregisterFromFixedUpdate(IFixedUpdatable fixedUpdatable, int updaterId)
        {
            if(_fixedUpdateUpdatersDictionary.TryGetValue(updaterId, out var updater))
            {
                Unregister(updater, fixedUpdatable);
            }
            else
            {
                throw new ArgumentException($"No Fixed Update Updater found with Id {updaterId}");
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
        
        private void Unregister<TUpdater, TListener>(TUpdater updater, TListener listener)
            where TListener : IUpdateMethodListener 
            where TUpdater : IUpdater<TListener>
        {
            if(_aboutToQuit)
                return;
            
            updater.Unregister(listener);
        }

        private void OnApplicationQuit()
        {
            _aboutToQuit = true;
        }
    }
}
