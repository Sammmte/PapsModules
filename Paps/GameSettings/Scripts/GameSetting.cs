using Paps.Logging;
using Paps.Optionals;
using Paps.Persistence;
using Paps.ValueReferences;
using SaintsField.Playa;
using System;
using Unity.Properties;
using UnityEngine;

[assembly: GeneratePropertyBagsForAssembly]

namespace Paps.GameSettings
{
    public abstract class GameSetting : ScriptableObject
    {
        public const string BASE_CREATE_ASSET_MENU = "Paps/Game Settings/";

        public static TGameSetting Create<TGameSetting, TValue>(TValue defaultValue) where TValue : IEquatable<TValue> where TGameSetting : GameSetting<TValue>
        {
            var newGameSetting = ScriptableObject.CreateInstance<TGameSetting>();

            newGameSetting.DefaultValue = defaultValue;

            return newGameSetting;
        }

        public abstract bool IsDirty { get; }
        public abstract bool IsDefault { get; }
        public abstract void Reset();
        public abstract void ResetToDefault();
        public abstract void CommitChange();
        internal abstract void Initialize(DataStorageReader<string> reader);
        internal abstract void Save(DataStorageWriter<string> writer);
    }

    public abstract class GameSetting<T> : GameSetting, IValueReferenceSource<T> where T : IEquatable<T>
    {
        

        [field: SerializeField, DontCreateProperty] public T DefaultValue { get; internal set; } // Could be set to "file" access modifier PathToCoreCLR

        [NonSerialized] private Optional<T> _tempValue;
        [field: NonSerialized] [ShowInInspector] public T Value { get; private set; }

        [ShowInInspector]
        public T ViewValue
        {
            get
            {
                if(_tempValue.HasValue)
                {
                    return _tempValue;
                }

                return Value;
            }
        }

        [ShowInInspector] public override sealed bool IsDirty => _tempValue.HasValue;
        [ShowInInspector] public override sealed bool IsDefault => Value.Equals(DefaultValue);

        public event Action<GameSetting<T>> OnChangeCommitted;
        public event Action<GameSetting<T>, ViewValueChangeReason> OnViewValueChanged;

        public sealed override void Reset()
        {
            if(!IsDirty)
                return;

            NotifyIfViewValueChanges(() =>
            {
                _tempValue = default;
            }, ViewValueChangeReason.Reset);
        }

        public sealed override void ResetToDefault()
        {
            if(IsDirty && _tempValue.Value.Equals(DefaultValue))
            {
                return;
            }

            InternalSetTempValue(DefaultValue, ViewValueChangeReason.ResetToDefault);
        }

        public void SetTempValue(T tempValue)
        {
            InternalSetTempValue(tempValue, ViewValueChangeReason.ExternalSet);
        }

        private void InternalSetTempValue(T tempValue, ViewValueChangeReason reason)
        {
            if(Value.Equals(tempValue))
            {
                NotifyIfViewValueChanges(() =>
                {
                    _tempValue = default;
                }, reason);
            }
            else
            {
                NotifyIfViewValueChanges(() =>
                {
                    _tempValue = tempValue;
                }, reason);
            }
        }

        public sealed override void CommitChange()
        {
            if(!IsDirty)
                return;

            Value = _tempValue;
            _tempValue = default;

            OnChangeCommitted?.Invoke(this);
        }

        private void NotifyIfViewValueChanges(Action action, ViewValueChangeReason reason)
        {
            var previousViewValue = ViewValue;

            action();

            if(!previousViewValue.Equals(ViewValue))
            {
                OnViewValueChanged?.Invoke(this, reason);
            }
        }

        internal sealed override void Initialize(DataStorageReader<string> reader)
        {
            if(reader.TryRead<T>(out var value))
            {
                Value = value;
            }
            else
            {
                Value = DefaultValue;
            }

            OnInitialized();
        }

        internal sealed override void Save(DataStorageWriter<string> writer)
        {
            writer.Write(Value);
        }

        protected virtual void OnInitialized() { }
    }
}
