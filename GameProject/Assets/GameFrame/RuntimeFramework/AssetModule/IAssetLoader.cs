using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Duskvern
{
    public interface IAssetLoader
    {
        UniTask<T> LoadAsync<T>(string key) where T : UnityEngine.Object;
        void Release(string key);
    }
}
