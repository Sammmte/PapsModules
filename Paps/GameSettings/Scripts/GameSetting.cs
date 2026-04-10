using Paps.Logging;
using Paps.Optionals;
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
        public abstract void Reset();
        public abstract void ResetToDefault();
        public abstract void CommitChange();
        public abstract Optional<GameSettingSaveInfo> GetSaveInfo();
        public abstract void Initialize(Optional<GameSettingSaveInfo> saveInfo = default);
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

        [ShowInInspector] public override bool IsDirty => _tempValue.HasValue;

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

        public sealed override Optional<GameSettingSaveInfo> GetSaveInfo()
        {
            if(Value.Equals(DefaultValue))
            {
                return default;
            }

            return new GameSettingSaveInfo<T>(Value);
        }

        public sealed override void Initialize(Optional<GameSettingSaveInfo> saveInfo = default)
        {
            if(saveInfo.HasValue)
            {
                if(saveInfo.Value is GameSettingSaveInfo<T> casted)
                {
                    Value = casted.Value;
                }
                else
                {
                    this.LogError($"Tried to initialize GameSetting of type {GetType().Name} with a save info of differet type {saveInfo.Value.GetType().Name}");
                    Value = DefaultValue;
                }
            }
            else
            {
                Value = DefaultValue;
            }

            OnInitialized();
        }

        protected virtual void OnInitialized() { }
    }

    public enum ViewValueChangeReason
    {
        ExternalSet,
        Reset,
        ResetToDefault
    }
}
