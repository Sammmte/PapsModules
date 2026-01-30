using SaintsField;
using SaintsField.Playa;
using System;
using UnityEngine;

namespace Paps.Update
{
    public class DefaultUnityFixedUpdateUpdater : MonoBehaviour, IUpdater<IFixedUpdatable>
    {
        [AboveButton(nameof(Enable), "Enable")]
        [AboveButton(nameof(Disable), "Disable")]
        [SerializeField] private UpdateSchema<IFixedUpdatable> _updateSchema;
        
        private bool HasListeners => _updateSchema.HasListeners();
        [NonSerialized, ShowInInspector, ReadOnly] private bool _manualEnabled;
        
        private void Awake()
        {
            _updateSchema.Initialize();

            _manualEnabled = enabled;
            UpdateEnabled();
        }

        public void Register(IFixedUpdatable listener)
        {
            _updateSchema.Register(listener);
            
            UpdateEnabled();
        }

        public void Register(IFixedUpdatable listener, int updateSchemaGroupId)
        {
            _updateSchema.Register(listener, updateSchemaGroupId);
            
            UpdateEnabled();
        }

        public void Unregister(IFixedUpdatable listener)
        {
            _updateSchema.Unregister(listener);

            UpdateEnabled();
        }

        public void Unregister(IFixedUpdatable listener, int updateSchemaGroupId)
        {
            _updateSchema.Unregister(listener, updateSchemaGroupId);

            UpdateEnabled();
        }

        public void Enable()
        {
            _manualEnabled = true;
            
            UpdateEnabled();
        }

        public void Disable()
        {
            _manualEnabled = false;
            
            UpdateEnabled();
        }

        private void FixedUpdate()
        {
            _updateSchema.Update();
        }

        private void UpdateEnabled()
        {
            enabled = HasListeners && _manualEnabled;
        }

        public void Dispose()
        {
            _updateSchema.Dispose();
        }
    }
}