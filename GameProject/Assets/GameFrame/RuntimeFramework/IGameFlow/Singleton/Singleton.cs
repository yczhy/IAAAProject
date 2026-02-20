// PersistentMonoSingleton.cs
using UnityEngine;

/// <summary>
/// 全局唯一且跨场景保留的 MonoBehaviour 单例
/// 适用于：AudioManager, GameManager, AdService 等
/// </summary>
public abstract class PersistentMonoSingleton<T> : MonoBehaviour where T : PersistentMonoSingleton<T>
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();
                if (_instance == null)
                {
                    GameObject go = new GameObject($"[{typeof(T).Name}] (Persistent)");
                    _instance = go.AddComponent<T>();
                    //DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = (T)this;
        // DontDestroyOnLoad(gameObject);
    }
}

/// <summary>
/// 当前场景内唯一的 MonoBehaviour 单例，不跨场景
/// 适用于：LevelManager, UIManager, BattleController 等
/// </summary>
public abstract class SceneMonoSingleton<T> : MonoBehaviour where T : SceneMonoSingleton<T>
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();
                if (_instance == null)
                {
                    GameObject go = new GameObject($"[{typeof(T).Name}] (Scene)");
                    _instance = go.AddComponent<T>();
                    // 注意：这里不调用 DontDestroyOnLoad
                }
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = (T)this;
        // 不设置 DontDestroyOnLoad → 场景切换时自动销毁
    }
}


/// <summary>
/// 普通 C# 单例基类（线程安全，懒加载）
/// 适用于不依赖 Unity 引擎功能的全局服务（如配置、数据缓存等）
/// </summary>
public abstract class Singleton<T> where T : class, new()
{
    private static T _instance;
    private static readonly object _lock = new object();

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new T();
                    }
                }
            }
            return _instance;
        }
    }

    // 可选：提供销毁接口（一般不需要）
    public static void DestroyInstance()
    {
        _instance = null;
    }
}