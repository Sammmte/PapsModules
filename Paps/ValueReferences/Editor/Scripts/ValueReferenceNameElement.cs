using UnityEditor;
using UnityEngine.UIElements;

namespace Paps.ValueReferences.Editor
{
    [UxmlElement]
    public partial class ValueReferenceNameElement : VisualElement
    {
        private Label _nameLabel;
        private TextField _renameField;
        private Button _renameButton;

        private ValueReferenceAsset _currentData;

        public void Initialize()
        {
            _nameLabel = this.Q<Label>("NameLabel");
            _renameField = this.Q<TextField>("RenameField");
            _renameButton = this.Q<Button>("RenameButton");

            _renameField.RegisterValueChangedCallback(ev => RenameAndHideRenameView(ev.newValue));
            _renameField.RegisterCallback<FocusOutEvent>(ev => HideRenameView());

            _renameButton.clicked += SwitchRenameView;
        }

        public void SetData(ValueReferenceAsset data)
        {
            CleanUp();

            _currentData = data;

            UpdateNameLabel();

            HideRenameView();
        }

        public void ShowRenameView()
        {
            _nameLabel.style.display = DisplayStyle.None;

            _renameField.SetValueWithoutNotify(_currentData.name);
            _renameField.style.display = DisplayStyle.Flex;
            _renameField.Focus();
        }

        public void HideRenameView()
        {
            _nameLabel.style.display = DisplayStyle.Flex;

            _renameField.style.display = DisplayStyle.None;
        }

        private void SwitchRenameView()
        {
            if(IsOnRenameView())
            {
                HideRenameView();
            }
            else
            {
                ShowRenameView();
            }
        }

        private bool IsOnRenameView() => _renameField.style.display == DisplayStyle.Flex;

        private void Rename(string newName)
        {
            newName = newName.Trim();

            if(_currentData.name == newName)
                return;

            var assetPath = AssetDatabase.GetAssetPath(_currentData);

            AssetDatabase.RenameAsset(assetPath, newName);

            UpdateNameLabel();
        }

        private void RenameAndHideRenameView(string newName)
        {
            Rename(newName);
            HideRenameView();
        }

        private void UpdateNameLabel()
        {
            _nameLabel.text = _currentData.name;
            _nameLabel.tooltip = _currentData.name;
        }

        public void CleanUp()
        {
            HideRenameView();
            _currentData = null;
        }
    }
}
