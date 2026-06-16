using UnityEngine;
using UnityEngine.UIElements;

namespace Paps.BuildVersion
{
    public class BuildVersionUI : MonoBehaviour
    {
        [SerializeField] private UIDocument _uiDocument;

        private Label _versionLabel;

        private void Awake()
        {
            _versionLabel = _uiDocument.rootVisualElement.Q<Label>("VersionLabel");

            _versionLabel.text = "Version " + Application.version;

            DontDestroyOnLoad(gameObject);
        }
    }
}
