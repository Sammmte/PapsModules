using UnityEngine;

namespace Paps.Update
{
    public abstract class DefaultUpdaterRunner<T> : MonoBehaviour where T : IUpdateMethodListener
    {
        protected UpdateSchema<T> UpdateSchema;

        public void Initialize(UpdateSchema<T> updateSchema)
        {
            UpdateSchema = updateSchema;

            enabled = false;
        }
    }
}