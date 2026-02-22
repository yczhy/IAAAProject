using Cysharp.Threading.Tasks;
using Duskvern;
using UnityEngine;

public static class SpawnUtil 
{
    /// <summary>
    /// 用于需要指定父物体，但又不需要关心位置/旋转
    /// worldPositionStays ：当你把物体设置为某个父物体的子物体时，要不要“保持原来的世界坐标”
    /// </summary>
    public static async UniTask<T> Spawn<T>(string key, Transform parent, bool worldPositionStays = false) where T : Component
    {
        var prefab = await AssetModule.Instance.LoadAsset<GameObject>(key);
        var instance = PoolUtil.Spawn(prefab,  parent, worldPositionStays);
        return instance.GetComponent<T>();
    }

    /// <summary>
    /// 通过组件生成 GameObject
    /// 用于要明确指定「生成位置 + 旋转」的场景
    /// </summary>
    public static async UniTask<T> Spawn<T>(string key, Vector3 position, Quaternion rotation, Transform parent = null) where T : Component
    {
        var prefab = await AssetModule.Instance.LoadAsset<GameObject>(key);
        var instance = PoolUtil.Spawn(prefab,  position, rotation, parent);
        return instance.GetComponent<T>();
    }

    /// <summary>
    /// 通过组件生成 GameObject
    /// 不关心父物体、不关心初始化位置/旋转、只要生成一个对象，后续自己处理
    /// </summary>
    public static async UniTask<T> Spawn<T>(string key) where T : Component
    {
        var prefab = await AssetModule.Instance.LoadAsset<GameObject>(key);
        var instance = PoolUtil.Spawn(prefab);
        return instance.GetComponent<T>();
    }

    /// <summary>
    /// 用于需要指定父物体，但又不需要关心位置/旋转
    /// worldPositionStays ：当你把物体设置为某个父物体的子物体时，要不要“保持原来的世界坐标”
    /// </summary>
    public static async UniTask<GameObject> Spawn(string key, Transform parent, bool worldPositionStays = false)
    {
        var prefab = await AssetModule.Instance.LoadAsset<GameObject>(key);
        var instance = PoolUtil.Spawn(prefab,  parent, worldPositionStays);
        return instance;
    }

    /// <summary>
    /// 通过组件生成 GameObject
    /// 用于要明确指定「生成位置 + 旋转」的场景
    /// </summary>
    public static async UniTask<GameObject> Spawn(string key, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        var prefab = await AssetModule.Instance.LoadAsset<GameObject>(key);
        var instance = PoolUtil.Spawn(prefab,  position, rotation, parent);
        return instance;
    }

    /// <summary>
    /// 通过组件生成 GameObject
    /// 不关心父物体、不关心初始化位置/旋转、只要生成一个对象，后续自己处理
    /// </summary>
    public static async UniTask<GameObject> Spawn(string key)
    {
        var prefab = await AssetModule.Instance.LoadAsset<GameObject>(key);
        var instance = PoolUtil.Spawn(prefab);
        return instance;
    }
}
