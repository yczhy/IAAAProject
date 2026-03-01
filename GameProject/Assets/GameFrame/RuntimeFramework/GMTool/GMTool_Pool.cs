using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Duskvern;
using NUnit.Framework;
using SRDebugger.Internal;

public partial class SROptions
{
    private List<GameObject> objA = new ();
    private List<GameObject> objB = new ();
    private List<GameObject> objC = new ();
    
    [System.ComponentModel.Category("对象池"), DisplayName("生成物体A")]
    public async void CreateObjA()
    {
        var prefab = await SpawnUtil.Spawn("Assets/GameFrame/AddressableAssets/Cube.prefab");
        objA.Add(prefab);
    }
    
    [System.ComponentModel.Category("对象池"), DisplayName("回收物体A")]
    public void DestroyObjA()
    {
        var instance = objA[0];
        objA.Remove(instance);
        PoolUtil.Despawn(instance);      
    }
     
    [System.ComponentModel.Category("对象池"), DisplayName("生成物体B")]
    public async void CreateObjB()
    {
        var prefab = await SpawnUtil.Spawn("Assets/GameFrame/AddressableAssets/Cube (1).prefab");
        objB.Add(prefab);
    }
    
    [System.ComponentModel.Category("对象池"), DisplayName("回收物体B")]
    public void DestroyObjB()
    {
        var instance = objB[0];
        objB.Remove(instance);
        PoolUtil.Despawn(instance);    
    }
    
    [System.ComponentModel.Category("对象池"), DisplayName("生成物体C")]
    public async void CreateObjC()
    {
        var prefab = await SpawnUtil.Spawn("Assets/GameFrame/AddressableAssets/Cube (2).prefab");
        objC.Add(prefab);
    }
    
    [System.ComponentModel.Category("对象池"), DisplayName("回收物体C")]
    public void DestroyObjC()
    {
        var instance = objC[0];
        objC.Remove(instance);
        PoolUtil.Despawn(instance);    
    }
    
    [System.ComponentModel.Category("对象池"), DisplayName("查看日志")]
    public void DestroyOb22jC()
    {
        DebugLogger.LogError("213123");
        Debug.LogError("00000");
    }
}
