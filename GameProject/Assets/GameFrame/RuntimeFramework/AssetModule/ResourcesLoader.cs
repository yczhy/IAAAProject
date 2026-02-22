using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Duskvern
{
    public class ResourcesLoader : IAssetLoader
    {
        private Dictionary<string, Object> cache = new();
        private Dictionary<string, int> refCount = new();
        private Dictionary<string, UniTask<Object>> loadingTasks = new();

        public async UniTask<T> LoadAsync<T>(string key) where T : Object
        {
            // 已加载
            if (cache.TryGetValue(key, out var cached))
            {
                refCount[key]++;
                return cached as T;
            }

            // 正在加载
            if (loadingTasks.TryGetValue(key, out var existingTask))
            {
                var result = await existingTask;
                refCount[key]++;
                return result as T;
            }

            // 创建新加载任务
            var task = LoadInternal<T>(key);
            loadingTasks[key] = task;

            var asset = await task;

            loadingTasks.Remove(key);

            if (asset != null)
            {
                cache[key] = asset;
                refCount[key] = 1;
            }

            return asset as T;
        }

        private async UniTask<Object> LoadInternal<T>(string key) where T : Object
        {
            // Resources 同步加载
            T asset = Resources.Load<T>(key);

            if (asset == null)
            {
                Debug.LogError($"Resources load failed: {key}");
                return null;
            }

            await UniTask.CompletedTask;
            return asset;
        }

        public void Release(string key)
        {
            if (!cache.ContainsKey(key))
                return;

            refCount[key]--;

            if (refCount[key] <= 0)
            {
                Resources.UnloadAsset(cache[key]);
                cache.Remove(key);
                refCount.Remove(key);
            }
        }
    }
}