using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace Duskvern
{
    [CreateAssetMenu(fileName = "AddressableKeyTable", menuName = "Duskvern/FrameConfig/AddressableKeyTable")]
    public class AddressableKeyTableConfig : SerializedScriptableObject
    {
        [Title("Addressable Key Dictionary")]
        [ShowInInspector, DictionaryDrawerSettings(KeyLabel = "Key", ValueLabel = "Asset")]
        public Dictionary<string, Object> keyDict = new Dictionary<string, Object>();

        [Title("Search Key")] [ShowInInspector, ReadOnly]
        private string _searchResult = "";

        public string SearchKey(string key)
        {
            if (keyDict.TryGetValue(key, out var asset))
            {
                _searchResult = asset.name;
                return asset.name;
            }
            else
            {
                _searchResult = "Not found";
                return null;
            }
        }

        // 编辑器调用刷新
        public void RefreshDictionary(Dictionary<string, Object> newDict)
        {
            keyDict = newDict;
        }

        [Button(ButtonSizes.Large)]
        private void ClearDictionary()
        {
            keyDict.Clear();
            _searchResult = "";
        }
    }
}