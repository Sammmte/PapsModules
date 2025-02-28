using Paps.UnityExtensions;
using UnityEngine;

namespace Paps.DevelopmentTools.Runtime
{
    public class DontDestroyOnLoad : MonoBehaviour
    {
        private void Awake()
        {
            gameObject.DontDestroyOnLoad();
        }
    }
}
