using System.IO;
using UnityEditor;
using UnityEngine.UIElements;

namespace Paps.Favorites
{
    [UxmlElement]
    public partial class ObjectSceneCellElement : VisualElement, IFavoriteProvider
    {
        private Label _label;

        public FavoriteObject Favorite { get; private set; }

        public void Initialize()
        {
            _label = this.Q<Label>("SceneLabel");
        }

        public void SetData(FavoriteObject favorite)
        {
            CleanUp();

            Favorite = favorite;
            
            Update();
        }

        public void Update()
        {
            if(Favorite.IsSceneObject)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(Favorite.Id.assetGUID);

                _label.text = Path.GetFileNameWithoutExtension(assetPath);
                _label.tooltip = assetPath;
            }
            else
            {
                _label.text = string.Empty;
                _label.tooltip = string.Empty;
            }
        }

        public void CleanUp()
        {
            Favorite = null;
        }
    }
}
