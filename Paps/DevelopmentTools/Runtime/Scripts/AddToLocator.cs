using Paps.GlobalProvisioning;
using UnityEngine;

namespace Paps.DevelopmentTools.Runtime
{
    public class AddToLocator : MonoBehaviour
    {
        [SerializeField] private Component _component;

        private void Awake()
        {
            Locator.Create(_component.GetType(), _component);
        }
    }
}
