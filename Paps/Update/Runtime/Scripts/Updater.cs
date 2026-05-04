using Paps.Optionals;
using System;
using UnityEngine;

namespace Paps.Update
{
    public abstract class Updater : ScriptableObject, IDisposable
    {
        [SerializeField] private bool _startEnabled;

        [NonSerialized] protected internal bool IsDisposed;
        [NonSerialized] private bool _initialized;
        [NonSerialized] private bool _isEnabled;
        public bool IsEnabled
        {
            get => _isEnabled;

            set
            {
                var wasEnabled = _isEnabled;

                _isEnabled = value;

                if(wasEnabled != _isEnabled)
                {
                    if(wasEnabled)
                    {
                        OnDisabled();
                    }
                    else
                    {
                        OnEnabled();
                    }
                }
            }
        }

        public abstract bool HasListeners { get; }

        public void Initialize()
        {
            if(_initialized)
                return;

            OnInternalInitialize();
            OnInitialize();

            IsEnabled = _startEnabled;

            _initialized = true;
        }
        
        
        public void Dispose()
        {
            if(IsDisposed)
                return;

            ThrowIfNotInitialized();

            OnDispose();

            IsDisposed = true;
        }

        protected internal void ThrowIfNotInitialized()
        {
            if(!_initialized)
                throw new InvalidOperationException($"Updater {name} is not initialized. The updater must be registered at the UpdateManager");
        }

        protected internal abstract void OnInternalInitialize();
        protected abstract void OnInitialize();
        protected abstract void OnEnabled();
        protected abstract void OnDisabled();
        protected abstract void OnDispose();
    }

    public abstract class Updater<T> : Updater where T : IUpdateMethodListener
    {
        [field: SerializeField] protected UpdateSchema<T> UpdateSchema { get; private set; }

        public sealed override bool HasListeners => UpdateSchema.HasListeners();

        protected internal sealed override void OnInternalInitialize()
        {
            UpdateSchema.Initialize();
        }

        public void Register(T listener, Optional<int> updateSchemaGroupId = default)
        {
            if(IsDisposed)
                return;

            ThrowIfNotInitialized();

            if(updateSchemaGroupId.HasValue)
                UpdateSchema.Register(listener, updateSchemaGroupId);
            else
                UpdateSchema.Register(listener);

            OnRegister(listener);
        }
        public void Unregister(T listener, Optional<int> updateSchemaGroupId = default)
        {
            if(IsDisposed)
                return;

            ThrowIfNotInitialized();

            if(updateSchemaGroupId.HasValue)
                UpdateSchema.Unregister(listener, updateSchemaGroupId);
            else
                UpdateSchema.Unregister(listener);

            OnUnregister(listener);
        }

        protected virtual void OnRegister(T listener) { }
        protected virtual void OnUnregister(T listener) { }
    }
}