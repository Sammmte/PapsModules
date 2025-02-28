using UnityEngine;

namespace Paps.DevelopmentTools.Runtime
{
    public class DisableIfProductionBuild : MonoBehaviour
    {
        private void Awake()
        {
#if PRODUCTION
            gameObject.SetActive(false);
#endif
        }
    }
}
