using System.Collections.Generic;
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
    public void CreateObjA()
    {
        var prefab = Resources.Load<GameObject>("Test/CubeA");
        objA.Add(PoolUtil.Spawn(prefab));
    }
    
    [System.ComponentModel.Category("对象池"), DisplayName("回收物体A")]
    public void DestroyObjA()
    {
        var instance = objA[0];
        objA.Remove(instance);
        PoolUtil.Despawn(instance);      
    }
     
    [System.ComponentModel.Category("对象池"), DisplayName("生成物体B")]
    public void CreateObjB()
    {
        var prefab = Resources.Load<GameObject>("Test/CubeB");
        objB.Add(PoolUtil.Spawn(prefab));
    }
    
    [System.ComponentModel.Category("对象池"), DisplayName("回收物体B")]
    public void DestroyObjB()
    {
        var instance = objB[0];
        objB.Remove(instance);
        PoolUtil.Despawn(instance);    
    }
    
    [System.ComponentModel.Category("对象池"), DisplayName("生成物体C")]
    public void CreateObjC()
    {
        var prefab = Resources.Load<GameObject>("Test/CubeC");
        objC.Add(PoolUtil.Spawn(prefab));
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
        DebugLogger.LogError("213123",false);
        Debug.LogError("00000");
    }
}
