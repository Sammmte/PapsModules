using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace Paps.Cheats
{
    public class CheatsUIDocument : MonoBehaviour
    {
        [SerializeField] private UIDocument _uIDocument;
        [SerializeField] private InputActionReference _toggleVisibilityAction;

        private CheatsUI _cheatsUI;

        public async UniTask Initialize()
        {
            _cheatsUI = _uIDocument.rootVisualElement.Q<CheatsUI>();

            _cheatsUI.Hide();

            await _cheatsUI.Initialize(_toggleVisibilityAction);
        }
    }
}
