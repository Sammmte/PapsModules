using UnityEngine;

namespace Paps.UnityExtensions
{
    public class Unparent : MonoBehaviour
    {
        [SerializeField] private Transform[] _transforms;

        private void Awake()
        {
            for (int i = 0; i < _transforms.Length; i++)
            {
                _transforms[i].parent = null;
            }
            
            Destroy(this);
        }
    }
}