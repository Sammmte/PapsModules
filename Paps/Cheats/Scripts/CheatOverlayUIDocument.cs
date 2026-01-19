using UnityEngine;
using UnityEngine.UIElements;

namespace Paps.Cheats
{
    public class CheatOverlayUIDocument : MonoBehaviour
    {
        [SerializeField] private UIDocument _uIDocument;
        [SerializeField] private VisualTreeAsset _elementContainerVTA;

        private CheatOverlaysScreenElement _screenElement;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public CheatOverlaysScreenElement Initialize()
        {
            _screenElement = _uIDocument.rootVisualElement.Q<CheatOverlaysScreenElement>();

            _screenElement.Initialize(_elementContainerVTA);

            return _screenElement;
        }
    }
}
