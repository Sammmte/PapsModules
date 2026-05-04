using UnityEngine;

namespace Paps.Update
{
    [CreateAssetMenu(menuName = "Paps/Update/Default Fixed Update Updater")]
    public sealed class DefaultFixedUpdateUpdater : DefaultUnityUpdater<IFixedUpdatable> { }
}