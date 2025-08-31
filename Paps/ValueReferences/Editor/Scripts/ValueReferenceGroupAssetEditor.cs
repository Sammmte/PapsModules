using Paps.UnityExtensions.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Paps.ValueReferences.Editor
{
    [CustomEditor(typeof(ValueReferenceGroupAsset))]
    public class ValueReferenceGroupAssetEditor : UnityEditor.Editor
    {
        [SerializeField] private VisualTreeAsset _editorVTA;
        [SerializeField] private VisualTreeAsset _valueReferenceGroupElementVTA;
        [SerializeField] private VisualTreeAsset _valueReferenceElementVTA;

        private Dictionary<string, Type> _valueReferenceTypes;
        private ValueReferenceGroupAsset _groupAssetTarget;
        private ValueReferenceGroupElement _rootGroupElement;
        
        public override VisualElement CreateInspectorGUI()
        {
            _groupAssetTarget = (ValueReferenceGroupAsset)target;
            _valueReferenceTypes = GetValueReferenceTypes();

            var editorElementInstance = _editorVTA.CloneTree();
            var rootContainer = editorElementInstance.Q("RootGroupContainer");

            var rootGroupElementParent = _valueReferenceGroupElementVTA.CloneTree();

            _rootGroupElement = rootGroupElementParent.Q<ValueReferenceGroupElement>();
            
            _rootGroupElement.Initialize(_groupAssetTarget, serializedObject.FindPropertyBakingField(nameof(_groupAssetTarget.Group)),
                _valueReferenceElementVTA, _valueReferenceGroupElementVTA,
                _valueReferenceTypes, false);
            
            rootContainer.Add(_rootGroupElement);

            return editorElementInstance;
        }

        private Dictionary<string, Type> GetValueReferenceTypes()
        {
            return TypeCache.GetTypesDerivedFrom(typeof(ValueReferenceAsset<>))
                .Where(type => type != typeof(ValueReferenceAsset<>))
                .ToDictionary(type =>
                {
                    var typeArgument = type.BaseType.GetGenericArguments().First();
                    
                    if (typeArgument == typeof(int))
                        return "Int";
                    if (typeArgument == typeof(float))
                        return "Float";
                    
                    return typeArgument.Name;
                }, type => type);
        }
    }
}
