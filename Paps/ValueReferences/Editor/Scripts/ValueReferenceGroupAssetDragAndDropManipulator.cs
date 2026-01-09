using UnityEditor;
using UnityEngine.UIElements;
using System.Linq;
using System;
using System.IO;

namespace Paps.ValueReferences.Editor
{
    internal class ValueReferenceGroupAssetDragAndDropManipulator : PointerManipulator
    {
        private VisualElement _dragAndDropRootElement;
        private Button _addButton;
        private Label _stateLabel;

        private string _normalLabelString;

        private Action<ValueReferenceAsset[]> _onAssetsAdded;

        public ValueReferenceGroupAssetDragAndDropManipulator(VisualElement dragAndDropRootElement, Action<ValueReferenceAsset[]> OnAssetsAdded)
        {
            target = dragAndDropRootElement;
            _onAssetsAdded = OnAssetsAdded;

            _addButton = target.Q<Button>("AddButton");
            _stateLabel = target.Q<Label>("DragAndDropLabel");

            _normalLabelString = _stateLabel.text;
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<DragEnterEvent>(OnDragEnter);
            target.RegisterCallback<DragLeaveEvent>(OnDragLeave);
            target.RegisterCallback<DragUpdatedEvent>(OnDragUpdated);
            target.RegisterCallback<DragPerformEvent>(OnDragPerformed);
            target.RegisterCallback<DragExitedEvent>(OnDragExited);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<DragEnterEvent>(OnDragEnter);
            target.UnregisterCallback<DragLeaveEvent>(OnDragLeave);
            target.UnregisterCallback<DragUpdatedEvent>(OnDragUpdated);
            target.UnregisterCallback<DragPerformEvent>(OnDragPerformed);
            target.UnregisterCallback<DragExitedEvent>(OnDragExited);
        }

        private void OnDragEnter(DragEnterEvent ev)
        {
            if(!IsValidDragAndDrop())
                return;

            string draggedName = null;
            
            if(DragAndDrop.objectReferences.Length > 0)
            {
                draggedName = GetDropLabelStringFromObjectReferences(DragAndDrop.objectReferences);
            }

            _stateLabel.text = $"Add {draggedName}";
            _addButton.enabledSelf = false;
        }

        private void OnDragLeave(DragLeaveEvent ev)
        {
            SetNormalState();
        }

        private void OnDragUpdated(DragUpdatedEvent ev)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
        }

        private void OnDragPerformed(DragPerformEvent ev)
        {
            if(!IsValidDragAndDrop())
                return;

            _onAssetsAdded(GetDroppedAsets());

            SetNormalState();
        }

        private void OnDragExited(DragExitedEvent ev)
        {
            SetNormalState();
        }
        
        private void SetNormalState()
        {
            _stateLabel.text = _normalLabelString;
            _addButton.enabledSelf = true;
        }

        private bool IsValidDragAndDrop()
        {
            return DragAndDrop.objectReferences.All(o => o is ValueReferenceAsset);
        }

        private ValueReferenceAsset[] GetDroppedAsets()
        {
            return DragAndDrop.objectReferences.Select(o => o as ValueReferenceAsset).ToArray();
        }

        private string GetDropLabelStringFromPaths(string[] paths)
        {
            string resultString = string.Empty;

            if(paths.Length == 1)
                return Path.GetFileNameWithoutExtension(paths[0]);

            for(int i = 0; i < paths.Length; i++)
            {
                resultString += Path.GetFileNameWithoutExtension(paths[i]);

                if(i != paths.Length - 1)
                {
                    resultString += ", ";
                }
            }

            return resultString;
        }

        private string GetDropLabelStringFromObjectReferences(UnityEngine.Object[] objects)
        {
            string resultString = string.Empty;

            if(objects.Length == 1)
                return objects[0].name;

            for(int i = 0; i < objects.Length; i++)
            {
                resultString += objects[i].name;

                if(i != objects.Length - 1)
                {
                    resultString += ", ";
                }
            }

            return resultString;
        }
    }
}
