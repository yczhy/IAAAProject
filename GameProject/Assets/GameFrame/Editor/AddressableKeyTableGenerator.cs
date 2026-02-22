using UnityEditor;
using UnityEngine;
using UnityEditor.AddressableAssets.Settings;
using System.Collections.Generic;
using Duskvern;
using UnityEditor.AddressableAssets;

namespace Duskvern
{
    public class AddressableKeyTableGenerator
    {
        [MenuItem("Tools/Duskvern/Generate Key Table")]
        public static void GenerateKeyTable()
        {
            // 获取 Addressable 设置
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                Debug.LogError("AddressableAssetSettings not found.");
                return;
            }

            // 查找或创建 ScriptableObject Key 表
            string path = "Assets/GameFrame/Editor/Configs/AddressableKeyTable.asset";
            var keyTable = AssetDatabase.LoadAssetAtPath<AddressableKeyTableConfig>(path);
            if (keyTable == null)
            {
                keyTable = ScriptableObject.CreateInstance<AddressableKeyTableConfig>();
                AssetDatabase.CreateAsset(keyTable, path);
                Debug.Log("Created new AddressableKeyTable at " + path);
            }

            // 扫描所有 Addressable 资源，生成字典
            var dict = new Dictionary<string, Object>();
            foreach (var group in settings.groups)
            {
                if (group == null) continue;
                foreach (var entry in group.entries)
                {
                    if (entry == null) continue;
                    var asset = AssetDatabase.LoadAssetAtPath<Object>(entry.AssetPath);
                    if (asset != null)
                    {
                        if (!dict.ContainsKey(entry.address))
                            dict[entry.address] = asset;
                        else
                            Debug.LogWarning($"Duplicate Addressable key found: {entry.address}");
                    }
                }
            }

            // 更新 Key 表
            keyTable.RefreshDictionary(dict);

            EditorUtility.SetDirty(keyTable);
            AssetDatabase.SaveAssets();

            Debug.Log($"AddressableKeyTable refreshed. Total entries: {dict.Count}");
        }
    }
}