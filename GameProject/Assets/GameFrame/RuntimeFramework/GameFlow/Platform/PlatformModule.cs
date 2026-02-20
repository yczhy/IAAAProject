using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Duskvern
{
    public class PlatformModule : PersistentMonoSingleton<PlatformModule>, IGameModule
    {
        private ChanngleConfig _channgleConfig;
        public void OnLoad()
        {
            _channgleConfig = ChanngleConfig.Instance;
        }

        public void Unload()
        {
            
        }

        public void OnPreLoad()
        {
            
        }

        public void OnPreUnload()
        {
            
        }

        public void OnModulePause()
        {
            
        }

        public void OnModuleResume()
        {
            
        }
    }
}

