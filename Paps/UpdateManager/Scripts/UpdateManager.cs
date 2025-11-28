using SaintsField;
using UnityEngine;

namespace Paps.UpdateManager
{
    public class UpdateManager : MonoBehaviour
    {
        public static UpdateManager Instance { get; private set; }
        
        [SerializeField] private SaintsInterface<IUpdater<IUpdatable>> _unityUpdateUpdater;
        [SerializeField] private SaintsInterface<IUpdater<ILateUpdatable>> _unityLateUpdateUpdater;
        [SerializeField] private SaintsInterface<IUpdater<IFixedUpdatable>> _unityFixedUpdateUpdater;

        private bool _aboutToQuit;

        private void Awake()
        {
            Instance = this;
            
            DontDestroyOnLoad(gameObject);
            
            _aboutToQuit = false;
        }

        public void RegisterForUpdate(IUpdatable updatable)
        {
            Register(_unityUpdateUpdater.I, updatable);
        }
        
        public void UnregisterFromUpdate(IUpdatable updatable)
        {
            Unregister(_unityUpdateUpdater.I, updatable);
        }
        
        public void RegisterForLateUpdate(ILateUpdatable lateUpdatable)
        {
            Register(_unityLateUpdateUpdater.I, lateUpdatable);
        }
        
        public void UnregisterFromLateUpdate(ILateUpdatable lateUpdatable)
        {
            Unregister(_unityLateUpdateUpdater.I, lateUpdatable);
        }
        
        public void RegisterForFixedUpdate(IFixedUpdatable fixedUpdatable)
        {
            Register(_unityFixedUpdateUpdater.I, fixedUpdatable);
        }
        
        public void UnregisterFromFixedUpdate(IFixedUpdatable fixedUpdatable)
        {
            Unregister(_unityFixedUpdateUpdater.I, fixedUpdatable);
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
