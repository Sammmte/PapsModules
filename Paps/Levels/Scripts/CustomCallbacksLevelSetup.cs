using Cysharp.Threading.Tasks;
using System;
using System.Threading;

namespace Paps.Levels
{
    public class CustomCallbacksLevelSetup : ILevelSetup
    {
        public Func<CancellationToken, UniTask> OnLoaded;
        public Func<CancellationToken, UniTask> OnSetup;
        public Action OnKickstart;
        public Action OnUnload;

        public async UniTask Load(CancellationToken cancellationToken)
        {
            if(OnLoaded != null)
            {
                await OnLoaded(cancellationToken);
            }
        }

        public async UniTask Setup(CancellationToken cancellationToken)
        {
            if(OnSetup != null)
            {
                await OnSetup(cancellationToken);
            }
        }

        public void Kickstart()
        {
            if(OnKickstart != null)
            {
                OnKickstart();
            }
        }

        public void Unload()
        {
            if(OnUnload != null)
            {
                OnUnload();
            }
        }
    }
}
