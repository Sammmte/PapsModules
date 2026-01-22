using System.IO;
using UnityEditor;
using UnityEngine.UIElements;

namespace Paps.Favorites
{
    [UxmlElement]
    public partial class ObjectCellElement : VisualElement, IFavoriteProvider
    {
        private Image _notPresentIcon;
        private Image _sceneObjectIcon;
        private Image _icon;
        private Label _objectNameLabel;
        private Label _sceneObjectNoticeLabel;

        public FavoriteObject Favorite { get; private set; }

        public void Initialize()
        {
            _icon = this.Q<Image>("Icon");
            _notPresentIcon = this.Q<Image>("ObjectNotPresentIcon");
            _sceneObjectIcon = this.Q<Image>("SceneObjectIcon");
            _objectNameLabel = this.Q<Label>("ObjectNameLabel");
            _sceneObjectNoticeLabel = this.Q<Label>("SceneObjectNoticeLabel");
        }

        public void SetData(FavoriteObject favorite)
        {
            CleanUp();

            Favorite = favorite;

            Update();
        }

        public void Update()
        {
            ShowCorrespondingIcon();

            if(Favorite.Object == null)
            {
                if(!string.IsNullOrEmpty(Favorite.LastKnownNameWithExtension))
                {
                    _objectNameLabel.text = Favorite.LastKnownNameWithExtension;
                }
                else
                {
                    _objectNameLabel.tooltip = "This object is not present anymore";
                    _objectNameLabel.text = "Unknown name";
                }
            }
            else
            {
                if(!Favorite.IsSceneObject)
                {
                    var assetPath = AssetDatabase.GetAssetPath(Favorite.Object);
                    _icon.image = AssetDatabase.GetCachedIcon(assetPath);
                    _objectNameLabel.text = Path.GetFileName(assetPath);
                }
                else
                {
                    _objectNameLabel.text = Favorite.Object.name;
                }
            }

            _sceneObjectNoticeLabel.style.display = Favorite.IsSceneObject ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public void CleanUp()
        {
            _icon.image = null;
            _objectNameLabel.text = string.Empty;
            _sceneObjectNoticeLabel.style.display = DisplayStyle.None;
        }

        private void ShowCorrespondingIcon()
        {
            if(Favorite.Object == null)
            {
                _notPresentIcon.style.display = DisplayStyle.Flex;
                _icon.style.display = DisplayStyle.None;
                _sceneObjectIcon.style.display = DisplayStyle.None;
                return;
            }

            if(Favorite.IsSceneObject)
            {
                _notPresentIcon.style.display = DisplayStyle.None;
                _icon.style.display = DisplayStyle.None;
                _sceneObjectIcon.style.display = DisplayStyle.Flex;
            }
            else
            {
                _notPresentIcon.style.display = DisplayStyle.None;
                _icon.style.display = DisplayStyle.Flex;
                _sceneObjectIcon.style.display = DisplayStyle.None;
            }
        }
    }
}
