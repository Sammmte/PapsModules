using UnityEngine.UIElements;
using System.Reflection;
using UnityEngine;
using System;
using Paps.Optionals;

namespace Paps.ValueReferences.Editor
{
    [UxmlElement]
    public partial class ValueReferenceTypeElement : VisualElement
    {
        [UxmlAttribute] private Color defaultColor;

        private Label _typeLabel;

        private ValueReferenceAsset _currentData;

        public void Initialize()
        {
            _typeLabel = this.Q<Label>("TypeLabel");
        }

        public void SetData(ValueReferenceAsset data)
        {
            CleanUp();

            _currentData = data;

            UpdateTypeLabel();
        }

        private void UpdateTypeLabel()
        {
            var valueProperty = _currentData.GetType().GetProperty("Value", BindingFlags.Public | BindingFlags.Instance);
            var type = valueProperty.PropertyType;

            var specialTreatment = GetCustomDisplayFor(type);
            
            if(specialTreatment.HasValue)
            {
                _typeLabel.text = specialTreatment.Value.DisplayName;
                _typeLabel.style.color = specialTreatment.Value.Color;
            }
            else
            {
                _typeLabel.text = type.Name;
                _typeLabel.tooltip = type.Name;
                _typeLabel.style.color = defaultColor;
            }
        }

        private Optional<ValueReferenceTypeCustomDisplay> GetCustomDisplayFor(Type type)
        {
            if(type == typeof(int))
            {
                return new ValueReferenceTypeCustomDisplay()
                {
                    DisplayName = "Int",
                    Color = Color.cyan,
                };
            }

            if(type == typeof(float))
            {
                return new ValueReferenceTypeCustomDisplay()
                {
                    DisplayName = "Float",
                    Color = Color.cyan,
                };
            }

            if(type == typeof(bool))
            {
                return new ValueReferenceTypeCustomDisplay()
                {
                    DisplayName = "Boolean",
                    Color = Color.yellow,
                };
            }

            if(type == typeof(string))
            {
                return new ValueReferenceTypeCustomDisplay()
                {
                    DisplayName = "String",
                    Color = Color.red,
                };
            }

            return default;
        }

        public void CleanUp()
        {
            _currentData = null;
        }
    }
}
