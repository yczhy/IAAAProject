using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Duskvern;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Duskvern
{
    public class UIModule : GameModule<UIModule>, IGameModule
    {
        [SerializeField] private UIConfig m_config; public UIConfig UIConfig => m_config;
        [SerializeField] private RectTransform m_panelRoot;
        [SerializeField] private RectTransform m_PanelPoolRoot;

        #region 流程方法

        /// <summary>
        /// 已经加载到的UI
        /// </summary>
        private Dictionary<string, UIPanel> m_uiPagesLoadedDic;

        /// <summary>
        /// 已经打开的页面
        /// </summary>
        private List<UIPanel> m_uiPanelOpenedList; public IReadOnlyList<UIPanel> GetAllPage() => m_uiPanelOpenedList;

        /// <summary>
        /// 不同层级的根结点
        /// </summary>
        private List<RectTransform> m_panelLayerRootList;

        private List<UIPanel> m_panelPoolList;

        public UIPanel GetPage(string pageName)
        {
            foreach (var page in m_uiPanelOpenedList)
            {
                if (page.UIPageType.Name == pageName)
                {
                    return page;
                }
            }

            return null;
        }

        public T GetPage<T>() where T : UIPanel
        {
            foreach (var page in m_uiPanelOpenedList)
            {
                if (page is T ret)
                {
                    return ret;
                }
            }

            return null;
        }

        private string curUIPageType = string.Empty;

        public override void OnLoad()
        {
            m_uiPagesLoadedDic = new Dictionary<string, UIPanel>();
            m_panelLayerRootList = new List<RectTransform>();
            m_uiPanelOpenedList = new List<UIPanel>();
            m_panelPoolList = new List<UIPanel>();

            if (m_config != null)
            {
                m_config.Init();
                var layerDef = m_config.LayerDef;
                for (int i = 0; i < layerDef.Count; i++)
                {
                    Transform layer = m_panelRoot.Find(layerDef[i].LayerName);
                    if (layer != null && layer.TryGetComponent<RectTransform>(out var rtf))
                    {
                        // 如果已经有层级节点 则直接使用
                        m_panelLayerRootList.Add(rtf);
                        continue;
                    }
                    GameObject layerRoot = new GameObject(m_config.LayerDef[i].LayerName);
                    RectTransform rt = layerRoot.AddComponent<RectTransform>();
                    rt.SetParent(m_panelRoot);
                    rt.anchoredPosition = Vector2.zero;
                    rt.localScale = Vector3.one;
                    rt.anchorMin = new Vector2(0, 0);
                    rt.anchorMax = new Vector2(1, 1);
                    rt.sizeDelta = new Vector2(0, 0);
                    rt.SetAsLastSibling();
                    m_panelLayerRootList.Add(rt);
                }
            }
        }

        public override void Unload()
        {
            // ReleaseAllPanel();
        }

        public override void OnPreLoad()
        {

        }

        public override void OnPreUnload()
        {

        }

        public override void OnModulePause()
        {

        }

        public override void OnModuleResume()
        {

        }

        #endregion

        #region 打开页面

        public async UniTask<TPanel> OpenPage<TPanel, TContext>(
            UIPageTypeBase<TPanel, TContext> uiPageType, TContext context, bool block = true)
            where TPanel : IUIPanel<TPanel, TContext>
            where TContext : IPanelContext
        {
            var panel = await _OpenPage(uiPageType.Name, context, block);
            if (panel is TPanel ret)
            {
                return ret;
            }
            return null;
        }

        private async UniTask<UIPanel> _OpenPage(string pageName, IPanelContext context, bool block)
        {
            if (!m_config || !m_config.UIConfigInfos.TryGetValue(pageName, out var configInfo))
            {
                return null;
            }
            UILayerSetting layerSetting = m_config.LayerDef[configInfo.Layer];
            if (!layerSetting.MultiPages || !configInfo.MultiPages) // 单例页面，如果已经打开了，就不打了
            {
                for (int i = m_uiPanelOpenedList.Count - 1; i > -1; i--)
                {
                    var page = m_uiPanelOpenedList[i];
                    if (page.UIPageType.Name == pageName)
                    {
                        if (!page.gameObject.activeSelf)
                        {
                            InitPanel(page, configInfo);
                        }

                        curUIPageType = string.Empty;
                        // 找到相同页面
                        await page.OpenPanel(context);
                        return page;
                    }
                }
            }

            await LoadPageAsync(pageName, autoInstance: false);
            if (!m_uiPagesLoadedDic.TryGetValue(pageName, out var pageBase) || pageBase == null)
            {
                curUIPageType = string.Empty;
                return null;
            }

            UIPanel pageBaseClone = null;
            for (int i = m_panelPoolList.Count - 1; i > -1; i--)
            {
                var cachePage = m_panelPoolList[i];
                if (cachePage != null && cachePage.UIPageType.Name == pageName)
                {
                    pageBaseClone = cachePage;
                    m_panelPoolList.RemoveAt(i);
                    pageBaseClone.transform.SetParent(m_panelLayerRootList[configInfo.Layer], false);
                    break;
                }
            }

            if (pageBaseClone == null)
            {
                pageBaseClone = Instantiate(pageBase, m_panelLayerRootList[configInfo.Layer]);
            }

            InitPanel(pageBaseClone, configInfo);
            await pageBaseClone.OpenPanel(context);
            curUIPageType = string.Empty;
            return pageBaseClone;
        }

        private void InitPanel(UIPanel pageBaseClone, UIConfigInfo configInfo)
        {
            pageBaseClone.RectTransform.anchoredPosition = Vector2.zero;
            pageBaseClone.RectTransform.localScale = Vector3.one;

            if (configInfo.ZIndex >= 1000)
            {
                pageBaseClone.RectTransform.SetAsLastSibling();
            }
            else
            {
                var layer = m_panelLayerRootList[configInfo.Layer];
                bool over = true;
                bool min = true;
                for (int i = layer.childCount - 1; i > -1; i--)
                {
                    var child = layer.GetChild(i);
                    if (child.TryGetComponent<UIPanel>(out var childPage))
                    {
                        if (childPage == pageBaseClone)
                        {
                            over = false;
                            continue;
                        }

                        var childConfig = m_config.UIConfigInfos[childPage.UIPageType.Name];
                        if (configInfo.ZIndex >= childConfig.ZIndex)
                        {
                            pageBaseClone.RectTransform.SetSiblingIndex(over ? i : i + 1);
                            min = false;
                            break;
                        }
                    }
                }

                if (min)
                {
                    pageBaseClone.RectTransform.SetAsFirstSibling();
                }
            }
            
            int index = configInfo.Index;
            m_uiPanelOpenedList.Add(pageBaseClone);
            pageBaseClone.gameObject.SetActive(true);
            // BizzaEventSystem.Emit(EventDefine.Frame.OpenPage, pageBaseClone.UIPageType.Name);
        }

        #endregion

        #region 加载卸载页面资源

        public async UniTask LoadPageAsync(string pageName, bool autoInstance = false)
        {
            if (!m_config || !m_config.UIConfigInfos.ContainsKey(pageName))
            {
                Debug.LogError("UIConfig does not have this page:" + pageName);
                return;
            }

            UIConfigInfo configInfo = m_config.UIConfigInfos[pageName];

            if (configInfo.PageReference.IsValid())
            {
                if (!configInfo.PageReference.IsDone)
                {
                    await configInfo.PageReference.OperationHandle.Task;
                }

                if (!m_uiPagesLoadedDic.ContainsKey(pageName))
                {
                    UIPanel pageBase = (configInfo.PageReference.Asset as GameObject).GetComponent<UIPanel>();
                    m_uiPagesLoadedDic.Add(pageName, pageBase);
                    if (autoInstance && configInfo.Cache)
                    {
                        var pageBaseClone = Instantiate(pageBase, m_panelLayerRootList[configInfo.Layer]);
                        pageBaseClone.gameObject.SetActive(false);
                        m_panelPoolList.Add(pageBaseClone);
                    }
                }
            }
            else
            {
                AsyncOperationHandle<GameObject> asyncOperation = configInfo.PageReference.LoadAssetAsync<GameObject>();
                await asyncOperation.Task;
                if (asyncOperation.Status != AsyncOperationStatus.Succeeded)
                {
                    return;
                }

                if (!m_uiPagesLoadedDic.ContainsKey(pageName))
                {
                    // ------ 加载完成
                    UIPanel pageBase = asyncOperation.Result.GetComponent<UIPanel>();
                    m_uiPagesLoadedDic.Add(pageName, pageBase);
                    if (autoInstance && configInfo.Cache)
                    {
                        var pageBaseClone = Instantiate(pageBase, m_panelLayerRootList[configInfo.Layer]);
                        pageBaseClone.gameObject.SetActive(false);
                        m_panelPoolList.Add(pageBaseClone);
                    }
                }
            }
        }

        public void ReleasePage(string pageName)
        {
            for (int i = m_uiPanelOpenedList.Count - 1; i > -1; i--)
            {
                if (m_uiPanelOpenedList[i].UIPageType.Name == pageName)
                {
                    Destroy(m_uiPanelOpenedList[i].gameObject);
                    m_uiPanelOpenedList.RemoveAt(i);
                }
            }

            if (m_config && m_config.UIConfigInfos.ContainsKey(pageName))
            {
                m_config.UIConfigInfos[pageName].PageReference.ReleaseAsset();
                m_uiPagesLoadedDic.Remove(pageName);
            }
        }

        #endregion


        #region 辅助方法

        public Canvas UICanvas
        {
            get
            {
                if (m_uiCanvas == null)
                    m_uiCanvas = m_panelRoot.GetComponentInParent<Canvas>();
                return m_uiCanvas;
            }
        }

        [SerializeField] private Canvas m_uiCanvas;
        public Canvas GetUICanvas()
        {
            if (m_uiCanvas == null)
                m_uiCanvas = m_panelRoot.GetComponentInParent<Canvas>();
            return m_uiCanvas;
        }

        public Camera UICamera
        {
            get { return UICanvas.GetComponentInChildren<Camera>(); }
        }

        #endregion
    }
}
