using System.Collections;
using System.Collections.Generic;
using Duskvern;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Duskvern
{
    [CreateAssetMenu(menuName = "Duskvern/FrameConfig/UIConfig")]
    public class UIConfig : ScriptableObject
    {
        [SerializeField, TableList] private List<UIConfigInfo> m_configs = new List<UIConfigInfo>(); public List<UIConfigInfo> Configs => m_configs;
        
        [Space, SerializeField, TableList] private List<UILayerSetting> m_layerDef = new List<UILayerSetting>(); public List<UILayerSetting> LayerDef => m_layerDef;

        public Dictionary<string, UIConfigInfo> UIConfigInfos { get; private set; }

        public void Init()
        {
            InitConfigDic();
        }
        
        private void InitConfigDic()
        {
            if (UIConfigInfos == null)
                UIConfigInfos = new ();
            UIConfigInfos.Clear();
            int infoLength = Configs.Count;
            for (int i = 0; i < infoLength; i++)
            {
                UIConfigInfos.Add(Configs[i].PageName, Configs[i]);
            }
        }
        
        public int GetLayerByName(string name)
        {
            for (int i = 0; i < LayerDef.Count; i++)
            {
                if (LayerDef[i].LayerName == name)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}


