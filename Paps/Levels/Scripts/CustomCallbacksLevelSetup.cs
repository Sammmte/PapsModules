using Cysharp.Threading.Tasks;
using System;

namespace Paps.Levels
{
    public class CustomCallbacksLevelSetup : ILevelSetup
    {
        public Func<UniTask> OnLoaded;
        public Func<UniTask> OnSetup;
        public Func<UniTask> OnKickstart;
        public Func<UniTask> OnUnload;

        public async UniTask Loaded()
        {
            if(OnLoaded != null)
            {
                await OnLoaded();
            }
        }

        public async UniTask Setup()
        {
            if(OnSetup != null)
            {
                await OnSetup();
            }
        }

        public async UniTask Kickstart()
        {
            if(OnKickstart != null)
            {
                await OnKickstart();
            }
        }

        public async UniTask Unload()
        {
            if(OnUnload != null)
            {
                await OnUnload();
            }
        }
    }
}
