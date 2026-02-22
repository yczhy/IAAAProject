using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Duskvern
{
    [System.Serializable] // 处理延迟回收的数据结构
    public class Delay : IPoolable
    {
        public GameObject clone;
        public float life;
        
        public bool IsInPool { get; set; }
        
        public void OnSpawn()
        {
            
        }

        public void OnDespawn()
        {
            life = 0f;
            clone = null;
        }
    }
    
    public enum E_NotificationType // 预制件生成时 的处理方式
    {
        /// <summary>
        /// 不发送通知，对象生成或回收时不执行任何额外方法，需要自己在 OnEnable/OnDisable 里处理逻辑
        /// </summary>
        None, 
        /// <summary>
        /// 发送消息 调用对象的 SendMessage("OnSpawn") / SendMessage("OnDespawn")，只作用于对象本身，不包含子对象
        /// </summary>
        //SendMessage, 
        /// <summary>
        /// 调用对象及其所有子对象的 BroadcastMessage("OnSpawn") / "OnDespawn"
        /// </summary>
        //BroadcastMessage,
        /// <summary>
        /// 对对象上实现了 IPoolable 接口的组件，调用它们的 OnSpawn() / OnDespawn() 方法
        /// </summary>
        IPoolable,
        /// <summary>
        /// 对对象及其子对象上实现了 IPoolable 的所有组件调用 OnSpawn() / OnDespawn()
        /// </summary>
        BroadcastIPoolable
    }
    
    /// <summary>
    /// 用于 控制对象池中回收（Despawn）对象的管理策略，也就是对象生成和回收时 对象的父级和激活状态如何处理
    /// </summary>
    public enum StrategyType
    {
        /// <summary>
        /// 对象被回收时调用 SetActive(false)，同时将对象放回到池的 Transform 下  
        /// 再生成时调用 SetActive(true) 并设置父级和位置  
        /// 常用于需要动态控制对象激活状态的场景（比如子弹、特效）
        /// </summary>
        ActivateAndDeactivate,
        /// <summary>
        /// 回收对象时不改变对象自身激活状态，而是将其放到一个 已经停用的父对象 下（DeactivatedChild）
        /// 不触碰对象的 SetActive，只靠父对象整体控制显隐
        /// 常用于保持对象原始激活状态或需要复杂层级管理的场景
        /// </summary>
        DeactivateViaHierarchy
    }
    
    [Serializable]
    public struct PoolConfig
    {
        [LabelText("通知类型")]
        public E_NotificationType eNotification;
        [LabelText("策略类型")]
        public StrategyType strategy;
        [LabelText("容量")]
        public int capacity;
        [LabelText("是否回收")]
        public bool recycle;
        [LabelText("是否持久化")]
        public bool persist;
        [LabelText("是否加索引")]
        public bool stamp;
        [LabelText("是否输出警告")]
        public bool warnings;
    }
    
    public enum E_ReleasePoolType
    {
        TransitionScene,
        All
    }
}