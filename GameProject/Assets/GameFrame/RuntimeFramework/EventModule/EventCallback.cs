using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Duskvern
{
    public interface IEventCallback
    {
        bool Empty();
        void Clear();
    }

    public class EventCallback : IEventCallback, IPoolable
    {
        private Action m_callback;
        private bool isInPool;
        public bool IsInPool
        {
            get => isInPool;
            set => isInPool = value;
        }

        public void Add(Action action)
        {
            m_callback += action;
        }

        public void Remove(Action action)
        {
            m_callback -= action;
        }

        public void Trigger()
        {
            m_callback?.Invoke();
        }

        public void Clear()
        {
            m_callback = null;
        }

        public void OnSpawn()
        {
            m_callback = null;
        }

        public void OnDespawn()
        {
            
        }

        public bool Empty()
        {
            return m_callback == null;
        }
    }

    public class EventCallback_1<T> : IEventCallback, IPoolable
    {
        private Action<T> m_callback;

        private bool isInPool;
        public bool IsInPool
        {
            get => isInPool;
            set => isInPool = value;
        }

        public void Add(Action<T> action)
        {
            m_callback += action;
        }

        public void Remove(Action<T> action)
        {
            m_callback -= action;
        }

        public void Trigger(T arg)
        {
            m_callback?.Invoke(arg);
        }

        public void Clear()
        {
            m_callback = null;
        }

        public void OnSpawn()
        {
            m_callback = null;
        }

        public void OnDespawn()
        {
            
        }
        public bool Empty()
        {
            return m_callback == null;
        }
    }

    public class EventCallback<T1, T2> : IEventCallback, IPoolable
    {
        private bool isInPool;
        public bool IsInPool
        {
            get => isInPool;
            set => isInPool = value;
        }

        private Action<T1, T2> m_callback;

        public void Add(Action<T1, T2> action)
        {
            m_callback += action;
        }

        public void Remove(Action<T1, T2> action)
        {
            m_callback -= action;
        }

        public void Trigger(T1 arg1, T2 arg2)
        {
            m_callback?.Invoke(arg1, arg2);
        }

        public void Clear()
        {
            m_callback = null;
        }

        public void OnSpawn()
        {
            m_callback = null;
        }

        public void OnDespawn()
        {
            
        }
        public bool Empty()
        {
            return m_callback == null;
        }
    }

    public class EventCallback<T1, T2, T3> : IEventCallback, IPoolable
    {
        private bool isInPool;
        public bool IsInPool
        {
            get => isInPool;
            set => isInPool = value;
        }
        private Action<T1, T2, T3> m_callback;

        public void Add(Action<T1, T2, T3> action)
        {
            m_callback += action;
        }

        public void Remove(Action<T1, T2, T3> action)
        {
            m_callback -= action;
        }

        public void Trigger(T1 arg1, T2 arg2, T3 arg3)
        {
            m_callback?.Invoke(arg1, arg2, arg3);
        }

        public void Clear()
        {
            m_callback = null;
        }

        public void OnSpawn()
        {
            m_callback = null;
        }

        public void OnDespawn()
        {
            
        }
        public bool Empty()
        {
            return m_callback == null;
        }
    }

    public class EventCallback<T1, T2, T3, T4> : IEventCallback, IPoolable
    {
        private bool isInPool;
        public bool IsInPool
        {
            get => isInPool;
            set => isInPool = value;
        }
        private Action<T1, T2, T3, T4> m_callback;

        public void Add(Action<T1, T2, T3, T4> action)
        {
            m_callback += action;
        }

        public void Remove(Action<T1, T2, T3, T4> action)
        {
            m_callback -= action;
        }

        public void Trigger(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            m_callback?.Invoke(arg1, arg2, arg3, arg4);
        }

        public void Clear()
        {
            m_callback = null;
        }

        public void OnSpawn()
        {
            m_callback = null;
        }

        public void OnDespawn()
        {
            
        }
        public bool Empty()
        {
            return m_callback == null;
        }
    }

    public class EventCallback<T1, T2, T3, T4, T5> : IEventCallback, IPoolable
    {
        private bool isInPool;
        public bool IsInPool
        {
            get => isInPool;
            set => isInPool = value;
        }
        private Action<T1, T2, T3, T4, T5> m_callback;

        public void Add(Action<T1, T2, T3, T4, T5> action)
        {
            m_callback += action;
        }

        public void Remove(Action<T1, T2, T3, T4, T5> action)
        {
            m_callback -= action;
        }

        public void Trigger(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            m_callback?.Invoke(arg1, arg2, arg3, arg4, arg5);
        }

        public void Clear()
        {
            m_callback = null;
        }

        public void OnSpawn()
        {
            m_callback = null;
        }

        public void OnDespawn()
        {
            
        }
        public bool Empty()
        {
            return m_callback == null;
        }
    }
}

