using Unity.Properties;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace Paps.DevelopmentTools.Editor
{
    [Overlay(typeof(SceneView), "Transform Extended", true)]
    public class TransformExtendedPanelOverlay : Overlay
    {
        private const string NO_OBJECT_SELECTED_LABEL = "No object selected";

        private TransformWorldDataSource _worldDataSource;
        private VisualElement _root;
        private Label _titleLabel;
        private Vector3Field _worldPositionField;
        private Vector3Field _worldRotationEulerField;
        private Vector3Field _worldScaleField;
        
        public override VisualElement CreatePanelContent()
        {
            _worldDataSource = new TransformWorldDataSource();
            _root = new VisualElement()
            {
                name = "TransformExtendedContainer",
                dataSource = _worldDataSource
            };
            _titleLabel = new Label();
            _worldPositionField = new Vector3Field("World Position");
            _worldRotationEulerField = new Vector3Field("World Rotation");
            _worldScaleField = new Vector3Field("World Scale");
            _worldScaleField.SetEnabled(false);
            
            _root.Add(_titleLabel);
            _root.Add(_worldPositionField);
            _root.Add(_worldRotationEulerField);
            _root.Add(_worldScaleField);

            UpdatePanelBasedOnSelection();

            Selection.selectionChanged += OnSelectionChanged;

            return _root;
        }

        public override void OnWillBeDestroyed()
        {
            Selection.selectionChanged -= OnSelectionChanged;
        }

        private void OnSelectionChanged()
        {
            UpdatePanelBasedOnSelection();
        }

        private void UpdatePanelBasedOnSelection()
        {
            var go = Selection.activeGameObject;
            
            if (go != null && PrefabStageUtility.GetPrefabStage(go) == null)
            {
                BindToObject(Selection.activeGameObject);
            }
            else
            {
                ShowEmpty();
            }
        }

        private void BindToObject(GameObject gameObject)
        {
            _worldDataSource.Set(gameObject);

            _titleLabel.text = gameObject.name;
            
            _worldPositionField.SetBinding(nameof(_worldPositionField.value), new DataBinding()
            {
                dataSourcePath = PropertyPath.FromName(nameof(_worldDataSource.WorldPosition))
            });

            _worldPositionField.style.display = DisplayStyle.Flex;
            
            _worldRotationEulerField.SetBinding(nameof(_worldRotationEulerField.value), new DataBinding()
            {
                dataSourcePath = PropertyPath.FromName(nameof(_worldDataSource.WorldRotationEuler))
            });
            
            _worldRotationEulerField.style.display = DisplayStyle.Flex;
            
            _worldScaleField.SetBinding(nameof(_worldScaleField.value), new DataBinding()
            {
                dataSourcePath = PropertyPath.FromName(nameof(_worldDataSource.WorldScale))
            });
            
            _worldScaleField.style.display = DisplayStyle.Flex;
        }

        private void ShowEmpty()
        {
            _titleLabel.text = NO_OBJECT_SELECTED_LABEL;
            
            _worldPositionField.ClearBindings();
            _worldRotationEulerField.ClearBindings();
            _worldScaleField.ClearBindings();

            _worldPositionField.style.display = DisplayStyle.None;
            _worldRotationEulerField.style.display = DisplayStyle.None;
            _worldScaleField.style.display = DisplayStyle.None;
            
            _worldDataSource.Clear();
        }
    }
}
