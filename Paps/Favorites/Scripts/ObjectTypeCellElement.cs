using UnityEngine.UIElements;

namespace Paps.Favorites
{
    [UxmlElement]
    public partial class ObjectTypeCellElement : VisualElement, IFavoriteProvider
    {
        private Label _typeLabel;

        public FavoriteObject Favorite { get; private set; }

        public void Initialize()
        {
            _typeLabel = this.Q<Label>("TypeLabel");
        }

        public void SetData(FavoriteObject favoriteObject)
        {
            CleanUp();

            Favorite = favoriteObject;

            Update();
        }

        public void Update()
        {
            if(Favorite.Object == null)
            {
                _typeLabel.text = "Unresolved";
            }
            else
            {
                _typeLabel.text = FavoriteTypeDisplayHelper.GetTypeDisplayName(Favorite.Object);
            }
        }

        public void CleanUp()
        {
            _typeLabel.text = string.Empty;

            Favorite = null;
        }
    }
}
