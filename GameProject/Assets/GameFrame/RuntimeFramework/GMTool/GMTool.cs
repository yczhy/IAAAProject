using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using SRDebugger.Services;
using SRF.Service;
using UnityEngine;

namespace Duskvern
{
    public class GMTool : PersistentMonoSingleton<GMTool>, IGameModule
    {
        private float _lastSwitchTime = 0;
        
        private void Update()
        {
            if (!ChanngleConfig.Instance.OpenGM) return;
            
            if ((Input.GetKeyDown(KeyCode.BackQuote) || Input.touchCount >= 5) &&
                Time.realtimeSinceStartup - _lastSwitchTime > 0.2f)
            {
                _lastSwitchTime = Time.realtimeSinceStartup;
                var mgr = SRServiceManager.GetService<IDebugService>();
                if (mgr != null)
                {
                    if (!mgr.IsDebugPanelVisible)
                    {
                        mgr.ShowDebugPanel();
                    }
                    else
                    {
                        mgr.HideDebugPanel();
                    }
                }
            }
        }

        public void OnLoad()
        {
            if (!ChanngleConfig.Instance.OpenGM) return;
            DealyShowPanel().Forget();
        }
        
        private async UniTask DealyShowPanel()
        {
            await UniTask.Delay(2000);
            SRDebug.Init();
            var mgr = SRServiceManager.GetService<IDebugService>();
            if (mgr != null)
            {
                mgr.ShowDebugPanel();
                mgr.HideDebugPanel();
                mgr.Settings.IsEnabled = ChanngleConfig.Instance.OpenGM;
            }
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