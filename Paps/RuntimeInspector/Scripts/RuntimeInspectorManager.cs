using RuntimeInspectorNamespace;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Inspector = RuntimeInspectorNamespace.RuntimeInspector;

namespace Paps.RuntimeInspector.Cheats
{
    public class RuntimeInspectorManager : MonoBehaviour
    {
        public static RuntimeInspectorManager Instance { get; private set; }

        [SerializeField] private GameObject _runtimeHierarchyContainer;
        [SerializeField] private GameObject _runtimeInspectorContainer;
        [SerializeField] private Button _hideHierarchyButton;
        [SerializeField] private Button _hideInspectorButton;
        [SerializeField] private RuntimeHierarchy _runtimeHierarchy;
        [SerializeField] private Inspector _runtimeInspector;

        public bool IsHierarchyEnabled => _runtimeHierarchyContainer.gameObject.activeSelf;

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            HideHierarchy();
            HideInspector();

            _hideHierarchyButton.onClick.AddListener(HideHierarchy);
            _hideInspectorButton.onClick.AddListener(HideInspector);

            _runtimeHierarchy.OnSelectionChanged += OnHierarchySelectionChanged;
        }

        public void HideHierarchy()
        {
            _runtimeHierarchyContainer.gameObject.SetActive(false);
        }

        public void HideInspector()
        {
            _runtimeInspector.StopInspect();
            _runtimeInspectorContainer.gameObject.SetActive(false);
        }

        public void ShowHierarchy()
        {
            _runtimeHierarchyContainer.gameObject.SetActive(true);
        }

        public void ShowInspector()
        {
            _runtimeInspectorContainer.gameObject.SetActive(true);
        }

        public void Inspect(object obj)
        {
            _runtimeInspectorContainer.gameObject.SetActive(true);
            _runtimeInspector.Inspect(obj);
        }

        private void OnHierarchySelectionChanged(IReadOnlyList<Transform> transforms)
        {
            if(transforms.Count > 0)
            {
                ShowInspector();
            }
        }
    }
}
