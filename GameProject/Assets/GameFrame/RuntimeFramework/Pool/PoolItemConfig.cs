using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Duskvern
{
    [CreateAssetMenu(fileName = "PoolItem.asset", menuName = "Duskvern/FrameConfig/PoolConfig")]
    public class PoolItemConfig : ScriptableObject
    {
        [SerializeField] // 记录当前已生成的对象（按顺序）-- 当 Recycle=true 时，需要知道生成顺序，方便回收最老对象
        private List<GameObject> spawnedClonesList = new List<GameObject>();

        public List<GameObject> SpawnedClonesList => spawnedClonesList;

        [SerializeField] // 记录当前生成的对象（无需顺序）
        private HashSet<GameObject> spawnedClonesHashSet = new HashSet<GameObject>();

        public HashSet<GameObject> SpawnedClonesHashSet => spawnedClonesHashSet;

        [SerializeField] // 延迟销毁的物体
        private List<Delay> delays = new List<Delay>();

        public List<Delay> Delays => delays;

        [SerializeField] // 存储已经回收的对象
        private List<GameObject> despawnedClones = new List<GameObject>();

        public List<GameObject> DespawnedClones => despawnedClones;

        [SerializeField, OnValueChanged("OnExaminePrefab")]
        private GameObject prefab; // 对象池的预制件

        public GameObject Prefab // 这个池子管理的原始 prefab
        {
            set
            {
                if (value != prefab)
                {
                    // 如果修改了 prefab，会先注销旧 prefab，然后注册新 prefab（保证 prefabMap 字典正确）
                    UnregisterPrefab();
                    prefab = value;
                    RegisterPrefab();
                }
            }
            get { return prefab; }
        }

        private void OnExaminePrefab()
        {
            if (!prefab.TryGetComponent(out IPoolable poolable))
            {
                DebugLogger.LogError("对象池的预制件必须实现 IPoolable", false);
                prefab = null;
            }
        }

        public string Name => prefab.name;

        [SerializeField] // 当使用 DeactivateViaHierarchy 策略时，回收对象挂载的停用父对象
        private Transform deactivatedTransform;

        [SerializeField] private Transform activatedTransform;

        // 获取停用父对象，如果不存在就创建一个 GameObject("Despawned Clones") 并停用
        public void SetCloneParent(GameObject clone)
        {
            switch (strategy)
            {
                case StrategyType.ActivateAndDeactivate:
                    if (activatedTransform == null) DebugLogger.LogError("PoolItemConfig.GetPoolParentTransform: activatedChild is null" + Name, false);
                    clone.SetActive(false);
                    clone.transform.SetParent(activatedTransform);
                    break;
                case StrategyType.DeactivateViaHierarchy:
                    if (deactivatedTransform == null) DebugLogger.LogError("PoolItemConfig.GetPoolParentTransform: deactivatedChild is null" + Name, false);
                    clone.transform.SetParent(deactivatedTransform);
                    break;
            }
        }

        #region 字段

        [SerializeField] // 对象池回收和取出的策略
        private NotificationType notification = NotificationType.None;

        public NotificationType Notification
        {
            set { notification = value; }
            get { return notification; }
        }

        [SerializeField] // 对象激活和隐藏的策略
        private StrategyType strategy = StrategyType.ActivateAndDeactivate;

        public StrategyType Strategy
        {
            set { strategy = value; }
            get { return strategy; }
        }

        [SerializeField] // 池子最大容量
        private int capacity;

        public int Capacity
        {
            set { capacity = value; }
            get { return capacity; }
        }

        [SerializeField] // 当池子满时，是否回收最老的对象以生成新对象
        private bool recycle;

        public bool Recycle
        {
            set { recycle = value; }
            get { return recycle; }
        }

        [SerializeField] // 是否 DontDestroyOnLoad
        private bool persist;

        public bool Persist
        {
            set { persist = value; }
            get { return persist; }
        }

        [SerializeField] // 生成的对象是否在名字上加上索引
        private bool stamp;

        public bool Stamp
        {
            set { stamp = value; }
            get { return stamp; }
        }

        [SerializeField] // 是否在控制台输出警告信息
        private bool warnings = true;

        public bool Warnings
        {
            set { warnings = value; }
            get { return warnings; }
        }

        public int Despawned => despawnedClones.Count; // 已经回收的对象数量
        public int Spawned => spawnedClonesList.Count + spawnedClonesHashSet.Count; // 记录当前生成的对象数量
        public int Total => Spawned + Despawned; // 总数量（生成 + 回收）

        #endregion

        private void Pop(ref GameObject go)
        {
            if (go.TryGetComponent<IPoolable>(out IPoolable poolable))
            {
                poolable.IsInPool = false;
            }
            else
            {
                DebugLogger.LogWarning("PoolItemConfig.GetPoolParentTransform: cannot find IPoolable", Name);
            }
        }

        #region 生成方法

        public GameObject Spawn()
        {
            var clone = default(GameObject);
            TrySpawn(ref clone);
            return clone;
        }

        public GameObject Spawn(Vector3 position)
        {
            var clone = default(GameObject);
            TrySpawn(ref clone, position);
            return clone;
        }

        public GameObject Spawn(Transform parent, bool worldPositionStays = false)
        {
            var clone = default(GameObject);
            TrySpawn(ref clone, parent, worldPositionStays);
            return clone;
        }

        public GameObject Spawn(Vector3 position, Quaternion rotation, Transform parent = null)
        {
            var clone = default(GameObject);
            TrySpawn(ref clone, position, rotation, parent);
            return clone;
        }

        public bool TrySpawn(ref GameObject clone, Transform parent, bool worldPositionStays = false)
        {
            if (prefab == null)
            {
                if (warnings == true)
                    Debug.LogWarning("You're attempting to spawn from a pool with a null prefab", this);
                return false;
            }

            if (parent != null && worldPositionStays == true)
            {
                return TrySpawn(ref clone, prefab.transform.position, Quaternion.identity, Vector3.one, parent,
                    worldPositionStays);
            }

            return TrySpawn(ref clone, prefab.transform.localPosition, prefab.transform.localRotation,
                prefab.transform.localScale, parent, worldPositionStays);
        }

        public bool TrySpawn(ref GameObject clone, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            if (prefab == null)
            {
                if (warnings == true)
                    Debug.LogWarning("You're attempting to spawn from a pool with a null prefab", this);
                return false;
            }

            if (parent != null)
            {
                position = parent.InverseTransformPoint(position);
                rotation = Quaternion.Inverse(parent.rotation) * rotation;
            }

            return TrySpawn(ref clone, position, rotation, prefab.transform.localScale, parent, false);
        }

        public bool TrySpawn(ref GameObject clone, Vector3 positon)
        {
            if (prefab == null)
            {
                if (warnings == true)
                    Debug.LogWarning("You're attempting to spawn from a pool with a null prefab", this);
                return false;
            }

            var transform = prefab.transform;
            return TrySpawn(ref clone, positon, transform.localRotation, transform.localScale, null, false);
        }

        public bool TrySpawn(ref GameObject clone)
        {
            if (prefab == null)
            {
                if (warnings == true)
                    Debug.LogWarning("You're attempting to spawn from a pool with a null prefab", this);
                return false;
            }

            var transform = prefab.transform;
            return TrySpawn(ref clone, transform.localPosition, transform.localRotation, transform.localScale, null,
                false);
        }

        public bool TrySpawn(ref GameObject clone, Vector3 localPosition, Quaternion localRotation, Vector3 localScale,
            Transform parent, bool worldPositionStays)
        {
            if (prefab == null)
            {
                if (warnings == true)
                    Debug.LogWarning("You're attempting to spawn from a pool with a null prefab", this);
                return false;
            }

            /*
             * 先从已经回收的列表中取
             * 再看一下当前生成的容量是否超标, >0 以及 回收 + 顺序 + 无序的数量
             * 最后，看一下是否支持 回收，先销毁最老的，再将最老的生成
             */
            for (var i = despawnedClones.Count - 1; i >= 0; i--) // 先从已经回收的物体中查找
            {
                clone = despawnedClones[i];
                despawnedClones.RemoveAt(i);
                if (clone != null)
                {
                    SpawnClone(clone, localPosition, localRotation, localScale, parent, worldPositionStays);
                    return true;
                }

                if (warnings == true)
                    Debug.LogWarning("This pool contained a null despawned clone, did you accidentally destroy it?",
                        this);
            }

            if (capacity <= 0 || Total < capacity) // 在范围内
            {
                clone = CreateClone(localPosition, localRotation, localScale, parent, worldPositionStays);
                if (recycle == true)
                {
                    spawnedClonesList.Add(clone);
                }
                else
                {
                    spawnedClonesHashSet.Add(clone);
                }

                if (strategy == StrategyType.ActivateAndDeactivate)
                {
                    clone.SetActive(true);
                }

                InvokeOnSpawn(clone);

                return true;
            }

            if (recycle == true && TryDespawnOldest(ref clone, false) == true)
            {
                SpawnClone(clone, localPosition, localRotation, localScale, parent, worldPositionStays);
                return true;
            }

            return false;
        }

        private void SpawnClone(GameObject clone, Vector3 localPosition, Quaternion localRotation, Vector3 localScale,
            Transform parent, bool worldPositionStays)
        {
            if (recycle == true)
            {
                spawnedClonesList.Add(clone);
            }
            else
            {
                spawnedClonesHashSet.Add(clone);
            }

            // Update transform
            var cloneTransform = clone.transform;

            cloneTransform.SetParent(null, false);

            cloneTransform.localPosition = localPosition;
            cloneTransform.localRotation = localRotation;
            cloneTransform.localScale = localScale;

            cloneTransform.SetParent(parent, worldPositionStays);

            // Make sure it's in the current scene
            if (parent == null)
            {
                SceneManager.MoveGameObjectToScene(clone, SceneManager.GetActiveScene());
            }

            // Activate
            if (strategy == StrategyType.ActivateAndDeactivate)
            {
                clone.SetActive(true);
            }

            // Notifications
            InvokeOnSpawn(clone);
        }

        private GameObject CreateClone(Vector3 localPosition, Quaternion localRotation, Vector3 localScale,
            Transform parent, bool worldPositionStays)
        {
            var clone = DoInstantiate(prefab, localPosition, localRotation, localScale, parent, worldPositionStays);

            if (stamp == true)
            {
                clone.name = prefab.name + " " + Total;
            }
            else
            {
                clone.name = prefab.name;
            }

            return clone;
        }

        private GameObject DoInstantiate(GameObject prefab, Vector3 localPosition, Quaternion localRotation,
            Vector3 localScale, Transform parent, bool worldPositionStays)
        {
#if UNITY_EDITOR
            if (Application.isPlaying == false && UnityEditor.PrefabUtility.IsPartOfRegularPrefab(prefab) == true)
            {
                if (worldPositionStays == true)
                {
                    return (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(prefab, parent);
                }
                else
                {
                    var clone = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(prefab, parent);

                    clone.transform.localPosition = localPosition;
                    clone.transform.localRotation = localRotation;
                    clone.transform.localScale = localScale;

                    return clone;
                }
            }
#endif

            if (worldPositionStays == true)
            {
                return Instantiate(prefab, parent, true);
            }
            else
            {
                var clone = Instantiate(prefab, localPosition, localRotation, parent);

                clone.transform.localPosition = localPosition;
                clone.transform.localRotation = localRotation;
                clone.transform.localScale = localScale;

                return clone;
            }
        }

        private void InvokeOnSpawn(GameObject clone)
        {
            if (clone == null)
            {
                DebugLogger.LogError("对象池物体为null" + Name, false);
            }

            switch (notification)
            {
                case NotificationType.IPoolable:
                    clone.GetComponents(tempPoolables);
                    for (var i = tempPoolables.Count - 1; i >= 0; i--)
                    {
                        tempPoolables[i].OnSpawn();
                        tempPoolables[i].IsInPool = false;
                    }

                    break;
                case NotificationType.BroadcastIPoolable:
                    clone.GetComponentsInChildren(tempPoolables);
                    for (var i = tempPoolables.Count - 1; i >= 0; i--)
                    {
                        tempPoolables[i].OnSpawn();
                        tempPoolables[i].IsInPool = false;
                    }

                    break;
            }
        }

        #endregion

        #region 回收方法

        public void DespawnOldest()
        {
            var clone = spawnedClonesList[0];
            TryDespawnOldest(ref clone, true);
        }

        private bool TryDespawnOldest(ref GameObject clone, bool registerDespawned)
        {
            MergeSpawnedClonesToList();
            var count = spawnedClonesList.Count;
            var index = count - 1;
            while (count > 0)
            {
                clone = spawnedClonesList[0];
                spawnedClonesList[0] = spawnedClonesList[index];
                spawnedClonesList.RemoveAt(index);
                if (clone != null)
                {
                    return true;
                }

                if (warnings == true)
                    Debug.LogWarning("This pool contained a null spawned clone, did you accidentally destroy it?",
                        this);
            }

            return false;
        }

        [Button("Despawn All"), PropertyOrder(-1)]
        public void DespawnAll()
        {
            DespawnAll(true);
        }

        public void DespawnAll(bool cleanLinks)
        {
            MergeSpawnedClonesToList();

            for (var i = spawnedClonesList.Count - 1; i >= 0; i--)
            {
                var clone = spawnedClonesList[i];

                if (clone != null)
                {
                    if (cleanLinks == true)
                    {
                        PoolUtil.Links.Remove(clone);
                    }
                }
            }

            spawnedClonesList.Clear();

            for (var i = delays.Count - 1; i >= 0; i--)
            {
                ClassPool<Delay>.Despawn(delays[i]);
            }

            delays.Clear();
        }

        public void Despawn(GameObject clone, float t = 0.0f)
        {
            if (clone != null)
            {
                if (!clone.TryGetComponent<IPoolable>(out var poolable))
                {
                    DebugLogger.LogError("clone没有挂载IPoolable脚本 " + clone.name, false);
                    return;
                }

                if (poolable.IsInPool)
                {
                    DebugLogger.LogError("反复将同一个物体放入对象池 " + clone.name, false);
                    return;
                }

                if (t > 0.0f)
                {
                    DespawnWithDelay(clone, t);
                }
                else
                {
                    TryDespawn(clone);
                    for (var i = delays.Count - 1; i >= 0; i--)
                    {
                        var delay = delays[i];

                        if (delay.clone == clone)
                        {
                            delays.RemoveAt(i);
                        }
                    }
                }
            }
            else
            {
                if (warnings == true) Debug.LogWarning("You're attempting to despawn a null gameObject", this);
            }
        }

        // 这里负责更新延时销毁的时间，如果对同一物体多次进行延迟销毁，取最快销毁的那一次
        private void DespawnWithDelay(GameObject clone, float t)
        {
            for (var i = delays.Count - 1; i >= 0; i--)
            {
                var delay = delays[i];

                if (delay.clone == clone)
                {
                    if (t < delay.life)
                    {
                        delay.life = t;
                    }

                    return;
                }
            }

            var newDelay = ClassPool<Delay>.Spawn(); // 这样进行对象的获取
            newDelay.clone = clone;
            newDelay.life = t;
            delays.Add(newDelay);
        }

        private void TryDespawn(GameObject clone)
        {
            if (spawnedClonesHashSet.Remove(clone) == true || spawnedClonesList.Remove(clone) == true)
            {
                DespawnNow(clone);
            }
            else
            {
                if (warnings == true)
                    Debug.LogWarning(
                        "You're attempting to despawn a GameObject that wasn't spawned from this pool, make sure your Spawn and Despawn calls match.",
                        clone);
            }
        }

        private void DespawnNow(GameObject clone, bool register = true)
        {
            if (register == true)
            {
                despawnedClones.Add(clone);
            }

            InvokeOnDespawn(clone);
        }

        private void InvokeOnDespawn(GameObject clone)
        {
            // IPoolable[] poolables = clone.GetComponents<IPoolable>();
            // if (poolables == null)
            // {
            //     DebugLogger.LogError("对象池物体未实现 IPoolable" + Name, false);
            // }

            // poolable.IsInPool = true;

            SetCloneParent(clone);

            switch (notification)
            {
                case NotificationType.IPoolable:
                    clone.GetComponents(tempPoolables);
                    for (var i = tempPoolables.Count - 1; i >= 0; i--)
                    {
                        tempPoolables[i].OnDespawn();
                        tempPoolables[i].IsInPool = true;
                    }

                    break;
                case NotificationType.BroadcastIPoolable:
                    clone.GetComponentsInChildren(tempPoolables);
                    for (var i = tempPoolables.Count - 1; i >= 0; i--)
                    {
                        tempPoolables[i].OnDespawn();
                        tempPoolables[i].IsInPool = true;
                    }

                    break;
            }
        }

        #endregion

        #region 剥离对象池

        public void Detach(GameObject clone, bool cleanLinks = true)
        {
            if (clone != null)
            {
                if (spawnedClonesHashSet.Remove(clone) == true || spawnedClonesList.Remove(clone) == true ||
                    despawnedClones.Remove(clone) == true)
                {
                    if (cleanLinks == true)
                    {
                        // Remove the link between this clone and this pool if it hasn't already been
                        PoolUtil.Links.Remove(clone);
                    }
                }
                else
                {
                    if (warnings == true)
                        Debug.LogWarning("You're attempting to detach a GameObject that wasn't spawned from this pool.",
                            clone);
                }
            }
            else
            {
                if (warnings == true) Debug.LogWarning("You're attempting to detach a null GameObject", this);
            }
        }

        #endregion

        #region 生命周期方法

        public PoolModule Pool { get; private set; }

        public Dictionary<GameObject, PoolItemConfig> prefabMap
        {
            get
            {
                if (Pool == null || Pool.PrefabMap == null)
                {
                    Debug.LogError("Pool is null", this);
                    return null;
                }

                return Pool.PrefabMap;
            }
        }

        public List<IPoolable> tempPoolables
        {
            get
            {
                if (Pool == null || Pool.TempPoolables == null)
                {
                    Debug.LogError("Pool is null", this);
                    return null;
                }

                return Pool.TempPoolables;
            }
        }

        public PoolItemConfig Init(PoolModule pool, GameObject _prefab, Transform _transform, PoolConfig _config)
        {
            if (pool == null || _transform == null)
            {
                Debug.LogError("对象池组件初始化有问题");
                return this;
            }

            this.Pool = pool;
            this.Prefab = _prefab;
            switch (_config.strategy)
            {
                case StrategyType.ActivateAndDeactivate: activatedTransform = _transform; break;
                case StrategyType.DeactivateViaHierarchy: deactivatedTransform = _transform; break;
            }

            {
                this.Notification = _config.notification;
                this.Strategy = _config.strategy;
                this.Capacity = _config.capacity;
                this.Recycle = _config.recycle;
                this.Persist = _config.persist;
                this.Stamp = _config.stamp;
                this.Warnings = _config.warnings;
            }

            return this;
        }

        public void Release()
        {
            this.activatedTransform = null;
            this.deactivatedTransform = null;

            DespawnAll();
        }

        public void OnUpdate(float delta)
        {
            for (var i = delays.Count - 1; i >= 0; i--)
            {
                var delay = delays[i];
                delay.life -= Time.deltaTime;
                if (delay.life > 0.0f) continue;

                delays.RemoveAt(i);
                ClassPool<Delay>.Despawn(delay);

                if (delay.clone != null)
                {
                    Despawn(delay.clone);
                }
                else
                {
                    if (warnings == true)
                        Debug.LogWarning(
                            "Attempting to update the delayed destruction of a prefab clone that no longer exists, did you accidentally destroy it?",
                            this);
                }
            }
        }

        private void RegisterPrefab()
        {
            if (prefab != null)
            {
                var existingPool = default(PoolItemConfig);

                if (prefabMap.TryGetValue(prefab, out existingPool) == true)
                {
                    Debug.LogWarning("You have multiple pools managing the same prefab (" + prefab.name + ").",
                        existingPool);
                }
                else
                {
                    prefabMap.Add(prefab, this);
                }
            }
        }

        private void UnregisterPrefab()
        {
            // Skip actually null prefabs, but allow destroyed prefabs
            if (Equals(prefab, null) == true)
            {
                return;
            }

            var existingPool = default(PoolItemConfig);
            if (prefabMap.TryGetValue(prefab, out existingPool) == true && existingPool == this)
            {
                prefabMap.Remove(prefab);
            }
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 获取生成队列中的元素
        /// </summary>
        /// <param name="gameObjects"></param>
        /// <param name="addSpawnedClones"></param>
        /// <param name="addDespawnedClones"></param>
        public void GetClones(List<GameObject> gameObjects, bool addSpawnedClones = true,
            bool addDespawnedClones = true)
        {
            if (gameObjects != null)
            {
                gameObjects.Clear();

                if (addSpawnedClones == true)
                {
                    gameObjects.AddRange(spawnedClonesList);
                    gameObjects.AddRange(spawnedClonesHashSet);
                }

                if (addDespawnedClones == true)
                {
                    gameObjects.AddRange(despawnedClones);
                }
            }
        }

        private void MergeSpawnedClonesToList()
        {
            if (spawnedClonesHashSet.Count > 0)
            {
                spawnedClonesList.AddRange(spawnedClonesHashSet);

                spawnedClonesHashSet.Clear();
            }
        }

        #endregion

        #region 功能测试

        [Button("Clean"), PropertyOrder(-1)]
        public void Clean()
        {
            for (var i = despawnedClones.Count - 1; i >= 0; i--)
            {
                DestroyImmediate(despawnedClones[i]);
            }

            despawnedClones.Clear();
        }

        #endregion
    }
}