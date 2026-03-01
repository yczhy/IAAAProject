using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Duskvern
{
    public class PoolModule : GameModule<PoolModule>, IGameModule
    {
        public override void OnUpdate(float deltaTime)
        {
            foreach (var _pool in PoolInstances)
            {
                _pool.OnUpdate(deltaTime);
            }
        }
    
        [SerializeField, LabelText("生成对象池模块的默认配置")] private PoolConfig defaultConfig;
        [ShowInInspector] private Dictionary<GameObject, PoolItemConfig> prefabMap; public Dictionary<GameObject, PoolItemConfig> PrefabMap => prefabMap;

        // 所有场景中激活的对象池模块
        public LinkedList<PoolItemConfig> PoolInstances; 
        
        // 当前模块中
        private LinkedListNode<PoolItemConfig> instancesNode;
        
        private static List<IPoolable> tempPoolables; public List<IPoolable> TempPoolables => tempPoolables;

        public override void OnLoad()
        {
            
        }

        public override void Unload()
        {
            ReleasePool(E_ReleasePoolType.All);
        }

        public override void OnPreLoad()
        {
            (prefabMap ??= new ()).Clear();
            (PoolInstances ??= new LinkedList<PoolItemConfig>()).Clear();
            (tempPoolables ??= new List<IPoolable>()).Clear();
        }

        public override void OnPreUnload()
        {
            
        }

        public override void OnModulePause()
        {
            
        }

        public override void OnModuleResume()
        {
            
        }
        
        public PoolItemConfig CreatePool(GameObject prefab)
        {
            PoolItemConfig pool = ScriptableObject.CreateInstance<PoolItemConfig>();
            pool.name = "PoolSO_" + prefab.name;
            GameObject root = new GameObject(pool.name + "_Root");
            root.transform.SetParent(transform);
            pool.Init(this, prefab, root.transform, defaultConfig);
            PoolInstances.AddLast(pool);
            return pool;
        }
        
        /// <summary>
        /// 根据类型释放对象池
        /// </summary>
        /// <param name="eReleaseType"></param>
        public void ReleasePool(E_ReleasePoolType eReleaseType)
        {
            switch (eReleaseType)
            {
                case E_ReleasePoolType.TransitionScene:
                {
                    foreach (var pool in PoolInstances)
                    {
                        if (pool.Persist) continue;
                        ReleasePool(pool);
                    }
                    break;
                }
                case E_ReleasePoolType.All:
                {
                    foreach (var pool in PoolInstances)
                    {
                        ReleasePool(pool);
                    }
                    break;
                }
            }
        }
        
        /// <summary>
        /// 释放指定类型的对象池
        /// </summary>
        /// <param name="pool"></param>
        public void ReleasePool(PoolItemConfig pool)
        {
            // TODO 这里的释放逻辑尽可能放到SO中
            if (pool == null)
            {
                Debug.LogError("释放的对象池为null");
                return;
            }

            pool.Release();
            prefabMap.Remove(pool.Prefab);
            PoolInstances.Remove(pool);
            Destroy(pool);
        }
        
        /// <summary>
        /// 通过特定 预制件 寻找对应的池子
        /// </summary>
        public bool TryFindPoolByPrefab(GameObject prefab, ref PoolItemConfig foundPool)
        {
            return prefabMap.TryGetValue(prefab, out foundPool);
        }
        
        public bool TryFindPoolByClone(GameObject clone, ref PoolItemConfig pool)
        {
            /// 为什么需要这个方法，而不是每个预制件中都保存池的引用 或者预制件的引用
            /// Prefab 是共享的，不是实例化的
            /// 实例化对象可能被任意操作 --- 有些 clone 可能被脱离池子（Detach）或者被外部 Destroy
            /// 果在每个实例上存池子引用，你每个对象都多了一个字段，序列化和内存开销都会增加，尤其是大量小对象（比如子弹、特效）时
            /// 有些对象可能从多个池子生成，或者临时生成而不属于任何池子
            foreach (var instance in PoolInstances)
            {
                if (instance.SpawnedClonesHashSet.Contains(clone) == true)
                {
                    pool = instance;

                    return true;
                }
            
                for (var j = instance.SpawnedClonesList.Count - 1; j >= 0; j--)
                {
                    if (instance.SpawnedClonesList[j] == clone)
                    {
                        pool = instance;

                        return true;
                    }
                }
            }
            return false;
        }
    }
}

