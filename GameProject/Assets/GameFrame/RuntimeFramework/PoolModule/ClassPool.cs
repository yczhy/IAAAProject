using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Duskvern
{
    /* 后续可以优化的内容
     *  可以分场景，
     *  将 onSpawn 修改为 工厂模式
     */
    public class ClassPool<T> where T : class, IPoolable, new()
    {
        private static Action<T> onSpawn;

        public static Action<T> OnSpawn
        {
            set => onSpawn = value;
        }

        private static int maxVolume = 100;
        public static int MaxVolume { get => maxVolume; set => maxVolume = value; }

        private static List<T> cache = new List<T>(); // 将所有已删除类的缓存存储于此处，以列表形式保存，以便我们能够进行搜索。
        
        private static T Create()
        {
            var instance = new T();
            instance.IsInPool = false;
            onSpawn?.Invoke(instance);
            return instance;
        }

        private static T Pop(int index)
        {
            var instance = cache[index];
            int count = cache.Count - 1;
            cache[index] = cache[count];
            cache.RemoveAt(count);
            instance.IsInPool = false;
            onSpawn?.Invoke(instance);
            return instance;
        }

        /// <summary>
        /// 直接取出一个对象，不做初始化处理
        /// </summary>
        /// <returns></returns>
        public static T Spawn()
        {
            var count = cache.Count;
            var index = count - 1;
            if (index >= 0)
            {
                return Pop(index);
            }
            else
            {
                return Create();
            }
        }
        
        /// <summary>
        /// 根据条件选择出合适的物体
        /// </summary>
        /// <param name="match"></param>
        /// <returns>如果找不到就会返回为null</returns>
        public static T Spawn(System.Predicate<T> match)
        {
            var instance = default(T);
            
            int count = cache.Count;
            if (count <= 0) return instance;
            int index = count - 1;
            for (; index >= 0; index--)
            {
                if (match(cache[index])) break;
            }
            
            if (index >= 0)
            {
                instance = Pop(index);
            }
            else
            {
                DebugLogger.LogInfo("未能找到符合条件的对象" + typeof(T).Name);
            }
            return instance;
        }
        
        /// <summary>
        /// 销毁对象
        /// </summary>
        /// <param name="instance"></param>
        public static void Despawn(T instance)
        {
            if (instance == null) return;
            if (cache.Count >= maxVolume)
            {
                DebugLogger.LogError($"超出池子最大容量" + typeof(T).Name);
                return; 
            }

            if (instance.IsInPool)
            {
                DebugLogger.LogError($"注意发生重复放入对象池的问题" + typeof(T).Name);
                return; 
            }
            instance.IsInPool = true;
            cache.Add(instance);
        }
        
        /// <summary>
        /// 销毁对象并执行指定方法
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="onDespawn"></param>
        private static void Despawn(T instance, System.Action<T> onDespawn)
        {
            if (instance != null)
            {
                onDespawn(instance);
                Despawn(instance);
            }
        }
    }
}