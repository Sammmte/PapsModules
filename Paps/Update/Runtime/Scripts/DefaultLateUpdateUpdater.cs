using UnityEngine;

namespace Paps.Update
{
    [CreateAssetMenu(menuName = "Paps/Update/Default Late Update Updater")]
    public sealed class DefaultLateUpdateUpdater : DefaultUnityUpdater<ILateUpdatable> { }
}