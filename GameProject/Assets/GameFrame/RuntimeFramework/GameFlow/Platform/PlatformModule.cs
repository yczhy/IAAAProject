using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Duskvern
{
    public class PlatformModule : PersistentMonoSingleton<PlatformModule>, IGameModule
    {
        private ChannleConfig _channgleConfig;
        public void OnLoad()
        {
            _channgleConfig = ChannleConfig.Instance;
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

        public void OnUpdate(float deltaTime)
        {
        
        }
    }
}

