using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Duskvern
{
    public abstract class ISaveDataConfig : ScriptableObject
    {
        public abstract bool HaveSave(string key);
        public abstract void Init();

        public abstract T Load<T>(string key, T defaultValue);

        public abstract void Save<T>(string key, T data);

        public abstract void Delete(string key);

        public abstract void DeleteAll();
    }

    public enum E_SaveType
    {
        Json,
        XML,
        Binary
    }
}
