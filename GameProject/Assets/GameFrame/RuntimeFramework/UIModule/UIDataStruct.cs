using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine.AddressableAssets;

namespace Duskvern
{
    public enum E_UIPanelType
    {
        None,
        UITest = 0,

    }
    
    /// <summary>
    /// 传递的参数
    /// </summary>
    public interface IPanelContext
    {
        void Print();
    }
    
    /// <summary>
    /// 页面的动画
    /// </summary>
    public interface IPanelTransition
    {
        UniTask PlayOpen(Transform target);
        UniTask PlayClose(Transform target);
    }
    
    [Serializable]
    public class UIConfigInfo 
    {
        [SerializeField, ReadOnly] private string m_uiPageName;
        [SerializeField, OnValueChanged("OnPageReferenceChange")]
        private AssetReferenceGameObject m_pageReference;
        [SerializeField] private bool m_multiPages = true;
        [SerializeField, ValueDropdown("GetLayerSelection")]
        private int m_layer;

        [SerializeField] private bool cache = true; public bool Cache { get { return cache; } }
        
        private const int IndexMax = 1000;
        private const int LayerIdx = IndexMax + 1;
        [SerializeField] [Range(0, IndexMax)] private int m_zIndex = 500;

        public int Index => m_zIndex * LayerIdx + m_layer;
        
        public AssetReferenceGameObject PageReference
        {
            get { return m_pageReference; }
            set { m_pageReference = value; }
        }
        
        public int Layer
        {
            get { return m_layer; }
            set { m_layer = value; }
        }
        
        public string PageName
        {
            get { return m_uiPageName; }
            set { m_uiPageName = value; }
        }
        
        public bool MultiPages => m_multiPages;
        public int ZIndex => m_zIndex;
        
        public bool IsValid
        {
            get
            {
                return m_pageReference != null && !string.IsNullOrEmpty(m_uiPageName);
            }
        }
        
        #if UNITY_EDITOR
        
        private void OnPageReferenceChange()
        {
            if (m_pageReference == null || m_pageReference.editorAsset == null)
                m_uiPageName = string.Empty;
            UIPanel pageBase = m_pageReference.editorAsset.GetComponent<UIPanel>();
            m_uiPageName = pageBase.UIPageType.Name;
        }
        
        private IEnumerable GetLayerSelection()
        {
            // UIConfig1 config = AssetDatabase.LoadAssetAtPath<UIConfig1>(UIConfig1.UIConfigAssetPath);
            var config = EditorUtils.FindObject<UIConfig>();
            if (config == null)
            {
                return null;
            }
            var valueDropList = new ValueDropdownList<int>();
            for (int i = 0; i < config.LayerDef.Count; i++)
            {
                valueDropList.Add(config.LayerDef[i].LayerName, i);
            }
        
            return valueDropList;
        }
        
        #endif
    }
    
    [System.Serializable]
    public class UILayerSetting
    {
        [SerializeField] private string m_layerName;
        [SerializeField] private bool m_multiPages = false;
        [SerializeField] private bool m_ignoreLowLayerManage = false;

        public string LayerName => m_layerName;
        public bool MultiPages => m_multiPages;

        public bool IgnoreLowLayerManage => m_ignoreLowLayerManage;
    }
}