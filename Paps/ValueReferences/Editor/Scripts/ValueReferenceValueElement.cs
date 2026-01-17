using UnityEngine.UIElements;
using EditorObject = UnityEditor.Editor;

namespace Paps.ValueReferences.Editor
{
    [UxmlElement]
    public partial class ValueReferenceValueElement : VisualElement
    {
        private VisualElement _editorContainer;
        private EditorObject _editorObject;
        private VisualElement _editorVisualElement;

        private ValueReferenceAsset _currentData;

        public void Initialize()
        {
            _editorContainer = this.Q("EditorContainer");
        }

        public void SetData(ValueReferenceAsset data)
        {
            CleanUp();

            _currentData = data;

            _editorObject = EditorObject.CreateEditor(_currentData);
            _editorVisualElement = _editorObject.CreateInspectorGUI();

            _editorContainer.Add(_editorVisualElement);
        }

        public void CleanUp()
        {
            _currentData = null;

            _editorContainer.Clear();
            _editorVisualElement = null;

            EditorObject.DestroyImmediate(_editorObject);
        }
    }
}
