using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Duskvern
{
    /// <summary>
    /// 游戏模块常驻场景内，不会被移除
    /// </summary>
    public abstract class GameModule<T> : PersistentMonoSingleton<T> where T : GameModule<T>, IGameModule
    {
        protected bool isLoaded = false;

        public abstract void OnLoad();
        public abstract void Unload();
        public abstract void OnPreLoad();
        public abstract void OnPreUnload();

        public abstract void OnModulePause();
        public abstract void OnModuleResume();

        public virtual void OnUpdate(float deltaTime) { }
    }
}