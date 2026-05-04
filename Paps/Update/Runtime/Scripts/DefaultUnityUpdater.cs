using UnityEngine;

namespace Paps.Update
{
    public abstract class DefaultUnityUpdater<T> : Updater<T> where T : IUpdateMethodListener
    {
        [SerializeField] private DefaultUpdaterRunner<T> _updateRunnerPrefab;

        private DefaultUpdaterRunner<T> _updateRunner;

        protected override void OnInitialize()
        {
            _updateRunner = Instantiate(_updateRunnerPrefab, UpdateManager.Instance.transform);

            _updateRunner.Initialize(UpdateSchema);
        }

        protected override void OnDisabled()
        {
            UpdateEnabled();
        }

        protected override void OnEnabled()
        {
            UpdateEnabled();
        }

        protected override void OnDispose()
        {
            _updateRunner.enabled = false;
        }

        private void UpdateEnabled()
        {
            _updateRunner.enabled = IsEnabled && HasListeners;
        }

        protected override void OnRegister(T listener)
        {
            UpdateEnabled();
        }

        protected override void OnUnregister(T listener)
        {
            UpdateEnabled();
        }
    }
}