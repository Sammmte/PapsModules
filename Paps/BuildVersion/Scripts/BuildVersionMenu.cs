using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;

namespace Paps.BuildVersion
{
    public class BuildVersionMenu : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        public static void Initialize()
        {
            var buildVersionPrefab = Addressables.LoadAssetAsync<GameObject>("BuildVersionMenu").WaitForCompletion();

            if (buildVersionPrefab == null)
                return;

            var buildVersionInstance = Instantiate(buildVersionPrefab).GetComponent<BuildVersionMenu>();
            DontDestroyOnLoad(buildVersionInstance);
        }

        [SerializeField] private UIDocument _uiDocument;

        private Label _versionLabel;

        private void Awake()
        {
            _versionLabel = _uiDocument.rootVisualElement.Q<Label>("VersionLabel");

            _versionLabel.text = "Version " + Application.version;
        }
    }
}
