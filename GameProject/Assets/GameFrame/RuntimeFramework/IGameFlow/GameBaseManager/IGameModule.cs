using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Duskvern
{
    public interface IGameModule
    {
        void OnLoad();
        void Unload();
        void OnPreLoad();
        void OnPreUnload();

        void OnModulePause();
        void OnModuleResume();
    }
}

