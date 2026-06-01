using Cysharp.Threading.Tasks;
using Paps.GameSetup;
using System.Threading;

namespace Paps.GameSettings
{
    public class GameSettingsGameSetupProcess : GameSetupProcess
    {
        public override async UniTask Setup(CancellationToken cancellationToken)
        {
            await GameSettingsManager.Instance.Initialize(cancellationToken);
        }
    }
}
