using UnityEditor;
using UnityEngine;
using UnityEditor.AddressableAssets.Settings;
using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using UnityEditor.AddressableAssets;

namespace Duskvern
{
    [CustomEditor(typeof(AddressableKeyTableConfig))]
    public class AddressableKeyTableEditor : OdinEditor
    {
        public override void OnInspectorGUI()
        {
            // DrawDefaultInspector();
            base.OnInspectorGUI(); // Odin 会绘制字典
            AddressableKeyTableConfig keyTableConfig = (AddressableKeyTableConfig)target;

            if (GUILayout.Button("Refresh Key Table"))
            {
                RefreshKeyTable(keyTableConfig);
            }

            GUILayout.Space(10);
            GUILayout.Label("Search Key", EditorStyles.boldLabel);
            string searchKey = EditorGUILayout.TextField("Key", "");
            if (!string.IsNullOrEmpty(searchKey))
            {
                string result = keyTableConfig.SearchKey(searchKey);
                EditorGUILayout.LabelField("Result", result);
            }
        }

        private void RefreshKeyTable(AddressableKeyTableConfig keyTableConfig)
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                Debug.LogError("AddressableAssetSettings not found.");
                return;
            }

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
                        dict[entry.address] = asset;
                    }
                }
            }

            keyTableConfig.RefreshDictionary(dict);
            EditorUtility.SetDirty(keyTableConfig);
            AssetDatabase.SaveAssets();
            Debug.Log($"AddressableKeyTable refreshed. Total entries: {dict.Count}");
        }
    }
}