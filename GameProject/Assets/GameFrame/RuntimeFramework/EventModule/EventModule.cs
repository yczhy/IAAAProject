using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Duskvern
{
    public enum E_EventType
    {
        All,
        Global,
        Local
    }

    public static class EventModule
    {
        private static bool warning = false;
        public static Dictionary<EventParm, IEventCallback> GlobalEventMap = new();
        public static Dictionary<EventParm, IEventCallback> LocalEventMap = new();

        #region 清空

        public static void Clear(E_EventType type)
        {
            switch (type)
            {
                case E_EventType.Global:
                    ClearMap(GlobalEventMap);
                    break;
                case E_EventType.Local:
                    ClearMap(LocalEventMap);
                    break;
                case E_EventType.All:
                    ClearMap(GlobalEventMap);
                    ClearMap(LocalEventMap);
                    break;
            }
        }

        private static void ClearMap(Dictionary<EventParm, IEventCallback> map)
        {
            foreach (var cb in map.Values)
            {
                if (cb == null) continue;
                cb.Clear();
                var t = cb.GetType();
                var poolType = typeof(ClassPool<>).MakeGenericType(t);
                var despawn = poolType.GetMethod("Despawn", new[] { t });
                despawn?.Invoke(null, new object[] { cb });
            }
            map.Clear();
        }

        #endregion

        #region 无参事件

        public static void AddG(EventParm parm, Action callback)
        {
            if (GlobalEventMap.TryGetValue(parm, out var ec))
            {
                if (ec is EventCallback ecl)
                {
                    ecl.Add(callback);
                }
                else
                {
                    DebugLogger.LogError("添加事件错误" + parm);
                }
            }
            else
            {
                EventCallback eventCallback = ClassPool<EventCallback>.Spawn();
                eventCallback.Add(callback);
                GlobalEventMap.Add(parm, eventCallback);
            }
        }

        public static void RemoveG(EventParm parm, Action callback)
        {
            if (GlobalEventMap.TryGetValue(parm, out var ec))
            {
                if (ec is EventCallback ecl)
                {
                    ecl.Remove(callback);
                    if (ecl.Empty())
                    {
                        GlobalEventMap.Remove(parm);
                        ClassPool<EventCallback>.Despawn(ecl);
                    }
                }
                else
                {
                    if (warning) DebugLogger.LogError("移除事件类型错误" + parm);
                }
            }
            else
            {
                if (warning) DebugLogger.LogError("移除事件错误" + parm);
            }
        }

        public static void TriggerG(EventParm parm)
        {
            if (GlobalEventMap.TryGetValue(parm, out var ec))
            {
                if (ec is EventCallback ecl)
                {
                    ecl.Trigger();
                }
                else
                {
                    if (warning) DebugLogger.LogError("触发事件错误" + parm);
                }
            }
        }

        public static void AddL(EventParm parm, Action callback)
        {
            if (LocalEventMap.TryGetValue(parm, out var ec))
            {
                if (ec is EventCallback ecl)
                {
                    ecl.Add(callback);
                }
                else
                {
                    DebugLogger.LogError("添加事件错误" + parm);
                }
            }
            else
            {
                EventCallback eventCallback = ClassPool<EventCallback>.Spawn();
                eventCallback.Add(callback);
                LocalEventMap.Add(parm, eventCallback);
            }
        }

        public static void RemoveL(EventParm parm, Action callback)
        {
            if (LocalEventMap.TryGetValue(parm, out var ec))
            {
                if (ec is EventCallback ecl)
                {
                    ecl.Remove(callback);
                }
                else
                {
                    if (warning) DebugLogger.LogError("移除事件类型错误" + parm);
                }
            }
            else
            {
                if (warning) DebugLogger.LogError("移除事件错误" + parm);
            }
        }

        public static void TriggerL(EventParm parm)
        {
            if (LocalEventMap.TryGetValue(parm, out var ec))
            {
                if (ec is EventCallback ecl)
                {
                    ecl.Trigger();
                }
                else
                {
                    if (warning) DebugLogger.LogError("触发事件错误" + parm);
                }
            }
        }

        #endregion

        #region 一参事件

        public static void AddG<T>(EventParm<T> parm, Action<T> callback)
        {
            if (GlobalEventMap.TryGetValue(parm, out var ec))
            {
                if (ec is EventCallback_1<T> ecl)
                {
                    ecl.Add(callback);
                }
                else
                {
                    DebugLogger.LogError("添加事件错误" + parm);
                }
            }
            else
            {
                EventCallback_1<T> eventCallback = ClassPool<EventCallback_1<T>>.Spawn();
                eventCallback.Add(callback);
                GlobalEventMap.Add(parm, eventCallback);
            }
        }

        public static void RemoveG<T>(EventParm<T> parm, Action<T> callback)
        {
            if (GlobalEventMap.TryGetValue(parm, out var ec))
            {
                if (ec is EventCallback_1<T> ecl)
                {
                    ecl.Remove(callback);
                    if (ecl.Empty())
                    {
                        GlobalEventMap.Remove(parm);
                        ClassPool<EventCallback_1<T>>.Despawn(ecl);
                    }
                }
                else
                {
                    if (warning) DebugLogger.LogError("移除事件类型错误" + parm);
                }
            }
            else
            {
                if (warning) DebugLogger.LogError("移除事件错误" + parm);
            }
        }

        public static void TriggerG<T>(EventParm<T> parm, T arg)
        {
            if (GlobalEventMap.TryGetValue(parm, out var ec))
            {
                if (ec is EventCallback_1<T> ecl)
                {
                    ecl.Trigger(arg);
                }
                else
                {
                    if (warning) DebugLogger.LogError("触发事件错误" + parm);
                }
            }
        }

        public static void AddL<T>(EventParm<T> parm, Action<T> callback)
        {
            if (LocalEventMap.TryGetValue(parm, out var ec))
            {
                if (ec is EventCallback_1<T> ecl)
                {
                    ecl.Add(callback);
                }
                else
                {
                    DebugLogger.LogError("添加事件错误" + parm);
                }
            }
            else
            {
                EventCallback_1<T> eventCallback = ClassPool<EventCallback_1<T>>.Spawn();
                eventCallback.Add(callback);
                LocalEventMap.Add(parm, eventCallback);
            }
        }

        public static void RemoveL<T>(EventParm<T> parm, Action<T> callback)
        {
            if (LocalEventMap.TryGetValue(parm, out var ec))
            {
                if (ec is EventCallback_1<T> ecl)
                {
                    ecl.Remove(callback);
                }
                else
                {
                    if (warning) DebugLogger.LogError("移除事件类型错误" + parm);
                }
            }
            else
            {
                if (warning) DebugLogger.LogError("移除事件错误" + parm);
            }
        }

        public static void TriggerL<T>(EventParm<T> parm, T arg)
        {
            if (LocalEventMap.TryGetValue(parm, out var ec))
            {
                if (ec is EventCallback_1<T> ecl)
                {
                    ecl.Trigger(arg);
                }
                else
                {
                    if (warning) DebugLogger.LogError("触发事件错误" + parm);
                }
            }
        }

        #endregion

        #region 二参事件

        public static void AddG<T1, T2>(EventParm<T1, T2> parm, Action<T1, T2> callback)
        {
            if (GlobalEventMap.TryGetValue(parm, out var ec))
            {
                if (ec is EventCallback<T1, T2> ecl)
                {
                    ecl.Add(callback);
                }
                else
                {
                    DebugLogger.LogError("添加事件错误" + parm);
                }
            }
            else
            {
                EventCallback<T1, T2> eventCallback = ClassPool<EventCallback<T1, T2>>.Spawn();
                eventCallback.Add(callback);
                GlobalEventMap.Add(parm, eventCallback);
            }
        }

        public static void RemoveG<T1, T2>(EventParm<T1, T2> parm, Action<T1, T2> callback)
        {
            if (GlobalEventMap.TryGetValue(parm, out var ec))
            {
                if (ec is EventCallback<T1, T2> ecl)
                {
                    ecl.Remove(callback);
                    if (ecl.Empty())
                    {
                        GlobalEventMap.Remove(parm);
                        ClassPool<EventCallback<T1, T2>>.Despawn(ecl);
                    }
                }
                else
                {
                    if (warning) DebugLogger.LogError("移除事件类型错误" + parm);
                }
            }
            else
            {
                if (warning) DebugLogger.LogError("移除事件错误" + parm);
            }
        }

        public static void TriggerG<T1, T2>(EventParm<T1, T2> parm, T1 arg1, T2 arg2)
        {
            if (GlobalEventMap.TryGetValue(parm, out var ec))
            {
                if (ec is EventCallback<T1, T2> ecl)
                {
                    ecl.Trigger(arg1, arg2);
                }
                else
                {
                    if (warning) DebugLogger.LogError("触发事件错误" + parm);
                }
            }
        }

        public static void AddL<T1, T2>(EventParm<T1, T2> parm, Action<T1, T2> callback)
        {
            if (LocalEventMap.TryGetValue(parm, out var ec))
            {
                if (ec is EventCallback<T1, T2> ecl)
                {
                    ecl.Add(callback);
                }
                else
                {
                    DebugLogger.LogError("添加事件错误" + parm);
                }
            }
            else
            {
                EventCallback<T1, T2> eventCallback = ClassPool<EventCallback<T1, T2>>.Spawn();
                eventCallback.Add(callback);
                LocalEventMap.Add(parm, eventCallback);
            }
        }

        public static void RemoveL<T1, T2>(EventParm<T1, T2> parm, Action<T1, T2> callback)
        {
            if (LocalEventMap.TryGetValue(parm, out var ec))
            {
                if (ec is EventCallback<T1, T2> ecl)
                {
                    ecl.Remove(callback);
                }
                else
                {
                    if (warning) DebugLogger.LogError("移除事件类型错误" + parm);
                }
            }
            else
            {
                if (warning) DebugLogger.LogError("移除事件错误" + parm);
            }
        }

        public static void TriggerL<T1, T2>(EventParm<T1, T2> parm, T1 arg1, T2 arg2)
        {
            if (LocalEventMap.TryGetValue(parm, out var ec))
            {
                if (ec is EventCallback<T1, T2> ecl)
                {
                    ecl.Trigger(arg1, arg2);
                }
                else
                {
                    if (warning) DebugLogger.LogError("触发事件错误" + parm);
                }
            }
        }

        #endregion

        #region 三参事件

        public static void AddG<T1, T2, T3>(EventParm<T1, T2, T3> parm, Action<T1, T2, T3> callback)
        {
            if (GlobalEventMap.TryGetValue(parm, out var ec))
            {
                if (ec is EventCallback<T1, T2, T3> ecl)
                {
                    ecl.Add(callback);
                }
                else
                {
                    DebugLogger.LogError("添加事件错误" + parm);
                }
            }
            else
            {
                EventCallback<T1, T2, T3> eventCallback = ClassPool<EventCallback<T1, T2, T3>>.Spawn();
                eventCallback.Add(callback);
                GlobalEventMap.Add(parm, eventCallback);
            }
        }

        public static void RemoveG<T1, T2, T3>(EventParm<T1, T2, T3> parm, Action<T1, T2, T3> callback)
        {
            if (GlobalEventMap.TryGetValue(parm, out var ec))
            {
                if (ec is EventCallback<T1, T2, T3> ecl)
                {
                    ecl.Remove(callback);
                    if (ecl.Empty())
                    {
                        GlobalEventMap.Remove(parm);
                        ClassPool<EventCallback<T1, T2, T3>>.Despawn(ecl);
                    }
                }
                else
                {
                    if (warning) DebugLogger.LogError("移除事件类型错误" + parm);
                }
            }
            else
            {
                if (warning) DebugLogger.LogError("移除事件错误" + parm);
            }
        }

        public static void TriggerG<T1, T2, T3>(EventParm<T1, T2, T3> parm, T1 arg1, T2 arg2, T3 arg3)
        {
            if (GlobalEventMap.TryGetValue(parm, out var ec))
            {
                if (ec is EventCallback<T1, T2, T3> ecl)
                {
                    ecl.Trigger(arg1, arg2, arg3);
                }
                else
                {
                    if (warning) DebugLogger.LogError("触发事件错误" + parm);
                }
            }
        }

        public static void AddL<T1, T2, T3>(EventParm<T1, T2, T3> parm, Action<T1, T2, T3> callback)
        {
            if (LocalEventMap.TryGetValue(parm, out var ec))
            {
                if (ec is EventCallback<T1, T2, T3> ecl)
                {
                    ecl.Add(callback);
                }
                else
                {
                    DebugLogger.LogError("添加事件错误" + parm);
                }
            }
            else
            {
                EventCallback<T1, T2, T3> eventCallback = ClassPool<EventCallback<T1, T2, T3>>.Spawn();
                eventCallback.Add(callback);
                LocalEventMap.Add(parm, eventCallback);
            }
        }

        public static void RemoveL<T1, T2, T3>(EventParm<T1, T2, T3> parm, Action<T1, T2, T3> callback)
        {
            if (LocalEventMap.TryGetValue(parm, out var ec))
            {
                if (ec is EventCallback<T1, T2, T3> ecl)
                {
                    ecl.Remove(callback);
                }
                else
                {
                    if (warning) DebugLogger.LogError("移除事件类型错误" + parm);
                }
            }
            else
            {
                if (warning) DebugLogger.LogError("移除事件错误" + parm);
            }
        }

        public static void TriggerL<T1, T2, T3>(EventParm<T1, T2, T3> parm, T1 arg1, T2 arg2, T3 arg3)
        {
            if (LocalEventMap.TryGetValue(parm, out var ec))
            {
                if (ec is EventCallback<T1, T2, T3> ecl)
                {
                    ecl.Trigger(arg1, arg2, arg3);
                }
                else
                {
                    if (warning) DebugLogger.LogError("触发事件错误" + parm);
                }
            }
        }

        #endregion

        #region 四参事件

        public static void AddG<T1, T2, T3, T4>(EventParm<T1, T2, T3, T4> parm, Action<T1, T2, T3, T4> callback)
        {
            if (GlobalEventMap.TryGetValue(parm, out var ec))
            {
                if (ec is EventCallback<T1, T2, T3, T4> ecl)
                {
                    ecl.Add(callback);
                }
                else
                {
                    DebugLogger.LogError("添加事件错误" + parm);
                }
            }
            else
            {
                EventCallback<T1, T2, T3, T4> eventCallback = ClassPool<EventCallback<T1, T2, T3, T4>>.Spawn();
                eventCallback.Add(callback);
                GlobalEventMap.Add(parm, eventCallback);
            }
        }

        public static void RemoveG<T1, T2, T3, T4>(EventParm<T1, T2, T3, T4> parm, Action<T1, T2, T3, T4> callback)
        {
            if (GlobalEventMap.TryGetValue(parm, out var ec))
            {
                if (ec is EventCallback<T1, T2, T3, T4> ecl)
                {
                    ecl.Remove(callback);
                    if (ecl.Empty())
                    {
                        GlobalEventMap.Remove(parm);
                        ClassPool<EventCallback<T1, T2, T3, T4>>.Despawn(ecl);
                    }
                }
                else
                {
                    if (warning) DebugLogger.LogError("移除事件类型错误" + parm);
                }
            }
            else
            {
                if (warning) DebugLogger.LogError("移除事件错误" + parm);
            }
        }

        public static void TriggerG<T1, T2, T3, T4>(EventParm<T1, T2, T3, T4> parm, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            if (GlobalEventMap.TryGetValue(parm, out var ec))
            {
                if (ec is EventCallback<T1, T2, T3, T4> ecl)
                {
                    ecl.Trigger(arg1, arg2, arg3, arg4);
                }
                else
                {
                    if (warning) DebugLogger.LogError("触发事件错误" + parm);
                }
            }
        }

        public static void AddL<T1, T2, T3, T4>(EventParm<T1, T2, T3, T4> parm, Action<T1, T2, T3, T4> callback)
        {
            if (LocalEventMap.TryGetValue(parm, out var ec))
            {
                if (ec is EventCallback<T1, T2, T3, T4> ecl)
                {
                    ecl.Add(callback);
                }
                else
                {
                    DebugLogger.LogError("添加事件错误" + parm);
                }
            }
            else
            {
                EventCallback<T1, T2, T3, T4> eventCallback = ClassPool<EventCallback<T1, T2, T3, T4>>.Spawn();
                eventCallback.Add(callback);
                LocalEventMap.Add(parm, eventCallback);
            }
        }

        public static void RemoveL<T1, T2, T3, T4>(EventParm<T1, T2, T3, T4> parm, Action<T1, T2, T3, T4> callback)
        {
            if (LocalEventMap.TryGetValue(parm, out var ec))
            {
                if (ec is EventCallback<T1, T2, T3, T4> ecl)
                {
                    ecl.Remove(callback);
                }
                else
                {
                    if (warning) DebugLogger.LogError("移除事件类型错误" + parm);
                }
            }
            else
            {
                if (warning) DebugLogger.LogError("移除事件错误" + parm);
            }
        }

        public static void TriggerL<T1, T2, T3, T4>(EventParm<T1, T2, T3, T4> parm, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            if (LocalEventMap.TryGetValue(parm, out var ec))
            {
                if (ec is EventCallback<T1, T2, T3, T4> ecl)
                {
                    ecl.Trigger(arg1, arg2, arg3, arg4);
                }
                else
                {
                    if (warning) DebugLogger.LogError("触发事件错误" + parm);
                }
            }
        }

        #endregion

        #region 五参事件

        public static void AddG<T1, T2, T3, T4, T5>(EventParm<T1, T2, T3, T4, T5> parm, Action<T1, T2, T3, T4, T5> callback)
        {
            if (GlobalEventMap.TryGetValue(parm, out var ec))
            {
                if (ec is EventCallback<T1, T2, T3, T4, T5> ecl)
                {
                    ecl.Add(callback);
                }
                else
                {
                    DebugLogger.LogError("添加事件错误" + parm);
                }
            }
            else
            {
                EventCallback<T1, T2, T3, T4, T5> eventCallback = ClassPool<EventCallback<T1, T2, T3, T4, T5>>.Spawn();
                eventCallback.Add(callback);
                GlobalEventMap.Add(parm, eventCallback);
            }
        }

        public static void RemoveG<T1, T2, T3, T4, T5>(EventParm<T1, T2, T3, T4, T5> parm, Action<T1, T2, T3, T4, T5> callback)
        {
            if (GlobalEventMap.TryGetValue(parm, out var ec))
            {
                if (ec is EventCallback<T1, T2, T3, T4, T5> ecl)
                {
                    ecl.Remove(callback);
                    if (ecl.Empty())
                    {
                        GlobalEventMap.Remove(parm);
                        ClassPool<EventCallback<T1, T2, T3, T4, T5>>.Despawn(ecl);
                    }
                }
                else
                {
                    if (warning) DebugLogger.LogError("移除事件类型错误" + parm);
                }
            }
            else
            {
                if (warning) DebugLogger.LogError("移除事件错误" + parm);
            }
        }

        public static void TriggerG<T1, T2, T3, T4, T5>(EventParm<T1, T2, T3, T4, T5> parm, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            if (GlobalEventMap.TryGetValue(parm, out var ec))
            {
                if (ec is EventCallback<T1, T2, T3, T4, T5> ecl)
                {
                    ecl.Trigger(arg1, arg2, arg3, arg4, arg5);
                }
                else
                {
                    if (warning) DebugLogger.LogError("触发事件错误" + parm);
                }
            }
        }

        public static void AddL<T1, T2, T3, T4, T5>(EventParm<T1, T2, T3, T4, T5> parm, Action<T1, T2, T3, T4, T5> callback)
        {
            if (LocalEventMap.TryGetValue(parm, out var ec))
            {
                if (ec is EventCallback<T1, T2, T3, T4, T5> ecl)
                {
                    ecl.Add(callback);
                }
                else
                {
                    DebugLogger.LogError("添加事件错误" + parm);
                }
            }
            else
            {
                EventCallback<T1, T2, T3, T4, T5> eventCallback = ClassPool<EventCallback<T1, T2, T3, T4, T5>>.Spawn();
                eventCallback.Add(callback);
                LocalEventMap.Add(parm, eventCallback);
            }
        }

        public static void RemoveL<T1, T2, T3, T4, T5>(EventParm<T1, T2, T3, T4, T5> parm, Action<T1, T2, T3, T4, T5> callback)
        {
            if (LocalEventMap.TryGetValue(parm, out var ec))
            {
                if (ec is EventCallback<T1, T2, T3, T4, T5> ecl)
                {
                    ecl.Remove(callback);
                }
                else
                {
                    if (warning) DebugLogger.LogError("移除事件类型错误" + parm);
                }
            }
            else
            {
                if (warning) DebugLogger.LogError("移除事件错误" + parm);
            }
        }

        public static void TriggerL<T1, T2, T3, T4, T5>(EventParm<T1, T2, T3, T4, T5> parm, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            if (LocalEventMap.TryGetValue(parm, out var ec))
            {
                if (ec is EventCallback<T1, T2, T3, T4, T5> ecl)
                {
                    ecl.Trigger(arg1, arg2, arg3, arg4, arg5);
                }
                else
                {
                    if (warning) DebugLogger.LogError("触发事件错误" + parm);
                }
            }
        }

        #endregion
    }
}
