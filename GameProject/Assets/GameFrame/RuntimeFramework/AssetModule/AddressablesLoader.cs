using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Duskvern
{
    /*
     * 后续需要做
     * 1、资源 + 实例双引用系统
     * 2、自动场景生命周期绑定
     * 3、Debug 资源面板
     * 4、Instantiate 出来的实例如何计数？
     * 5、场景卸载如何自动释放？
     * 6、Label 批量加载如何处理？
     * 7、依赖资源释放是否会误释放？
     */
    public class AddressablesLoader : IAssetLoader
    {
        private Dictionary<string, AsyncOperationHandle> handles = new();
        private Dictionary<string, int> refCount = new();
        private Dictionary<string, UniTask<Object>> loadingTasks = new();

        public async UniTask<T> LoadAsync<T>(string key) where T : Object
        {
            // 已加载
            if (handles.TryGetValue(key, out var existingHandle))
            {
                refCount[key]++;
                return existingHandle.Result as T;
            }

            // 正在加载
            if (loadingTasks.TryGetValue(key, out var existingTask))
            {
                var result = await existingTask;
                refCount[key]++;
                return result as T;
            }

            // 创建新任务
            var task = LoadInternal<T>(key);
            loadingTasks[key] = task;

            var asset = await task;

            loadingTasks.Remove(key);

            if (asset != null)
            {
                refCount[key] = 1;
            }

            return asset as T;
        }

        private async UniTask<Object> LoadInternal<T>(string key) where T : Object
        {
            var handle = Addressables.LoadAssetAsync<T>(key);
            await handle.ToUniTask();

            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"Addressables load failed: {key}");
                return null;
            }

            handles[key] = handle;
            return handle.Result;
        }

        public void Release(string key)
        {
            if (!handles.ContainsKey(key))
                return;

            refCount[key]--;

            if (refCount[key] <= 0)
            {
                Addressables.Release(handles[key]);
                handles.Remove(key);
                refCount.Remove(key);
            }
        }
    }
}