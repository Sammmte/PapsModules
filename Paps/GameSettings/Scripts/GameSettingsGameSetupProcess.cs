using Cysharp.Threading.Tasks;
using Paps.GameSetup;

namespace Paps.GameSettings
{
    public class GameSettingsGameSetupProcess : GameSetupProcess
    {
        public override async UniTask Setup()
        {
            await GameSettingsManager.Instance.Initialize();
        }
    }
}
