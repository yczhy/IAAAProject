using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Duskvern
{
    public class GameBootstrap : MonoBehaviour
    {
        [SerializeField] private GameObject[] m_preLoadModules;

        private IGameModule[] allModules;
    
        /// <summary>
        /// 这里作为全局的启动器  --- // 加载流程 -> 初始化SDK -> 初始化游戏模块 -> 进入游戏
        /// </summary>
        private void Awake()
        {
            IGameModule[] modules = new IGameModule[m_preLoadModules.Length];
            for (int i = 0; i < m_preLoadModules.Length; i++)
            {
                var module = m_preLoadModules[i].GetComponent<IGameModule>();
                if (module == null)
                {
                    DebugLogger.LogError("Game Module doesn't exist: " + m_preLoadModules[i].name);
                    return;
                }
                modules[i] = module;
            }

            OnLoadModules(modules);
            DontDestroyOnLoad(this);
        }
    
        /// <summary>
        /// 这里要加载各个模块，并且初始化
        /// </summary>
        private void Start()
        {
            allModules = transform.GetComponentsInChildren<IGameModule>();
            OnLoadModules(allModules);
        }

        private void OnLoadModules(IGameModule[] _modules)
        {
            foreach (var module in _modules)
            {
                module.OnPreLoad();
            }

            foreach (var module in _modules)
            {
                module.OnLoad();
            }
        }

        private void OnUnLoadModules(IGameModule[] _modules)
        {
            foreach (var module in _modules)
            {
                module.OnPreUnload();
            }

            foreach (var module in _modules)
            {
                module.Unload();
            }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
        
        }

        private void OnApplicationQuit()
        {
            DebugLogger.LogInfo("退出游戏");
            OnUnLoadModules(allModules);
        }
    }

}
