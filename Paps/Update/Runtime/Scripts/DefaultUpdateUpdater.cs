using UnityEngine;

namespace Paps.Update
{
    [CreateAssetMenu(menuName = "Paps/Update/Default Update Updater")]
    public sealed class DefaultUpdateUpdater : DefaultUnityUpdater<IUpdatable> { }
}