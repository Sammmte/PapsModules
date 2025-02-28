﻿using Cysharp.Threading.Tasks;

namespace Paps.LevelSetup
{
    public interface ILevelSetuppable
    {
        public UniTask Setup()
        {
            return UniTask.CompletedTask;
        }

        public void Kickstart()
        {

        }

        public UniTask Unload()
        {
            return UniTask.CompletedTask;
        }
    }
}
