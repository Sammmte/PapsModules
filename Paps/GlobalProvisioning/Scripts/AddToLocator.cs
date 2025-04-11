using UnityEngine;

namespace Paps.GlobalProvisioning
{
    public class AddToLocator : MonoBehaviour
    {
        [SerializeField] private Component _component;

        private void Awake()
        {
            Locator.Create(_component.GetType(), _component);
        }

        private void OnDestroy()
        {
            Locator.Remove(_component.GetType());
        }
    }
}
