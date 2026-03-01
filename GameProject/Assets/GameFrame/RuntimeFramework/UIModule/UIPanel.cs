using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Duskvern
{
    [RequireComponent(typeof(RectTransform))]
    public abstract class IUIBasePanel : MonoBehaviour
    {
        public abstract E_UIPanelType PanelType { get; } // 具体的页面类型
        public abstract UIPageTypeBase UIPageType { get; } // 页面标识
        protected IPanelContext m_panelContext;
        
        [SerializeField] protected IPanelTransition m_transition; // 页面过渡
        
        private void Awake()
        {
            m_rectTransform = transform as RectTransform;
        }

        protected virtual void OnAwake() { }
        
        private RectTransform m_rectTransform; public RectTransform RectTransform => m_rectTransform;

        protected abstract void OnShow();
        protected abstract void OnHide();
        protected abstract void OnClose();

        public async UniTask ClosePanel()
        {
            try
            {
                OnHide(); 
                if (m_transition != null) await m_transition.PlayClose(m_rectTransform);
                OnClose();
            }
            catch (Exception e)
            {
                DebugLogger.LogError($"关闭{UIPageType.ToString()}界面报错 {e.ToString()};", false);
            }
        }
    }
    
    public abstract class UIPanel : IUIBasePanel
    {
        protected abstract void OnOpen(IPanelContext panelContext);

        public async UniTask OpenPanel(IPanelContext panelContext)
        {
            try
            {
                OnShow();
                if (m_transition != null) await m_transition.PlayOpen(RectTransform);
                m_panelContext = panelContext;
                OnOpen(panelContext);
            }
            catch (Exception e)
            {
                DebugLogger.LogError($"打开{UIPageType.ToString()}界面报错 {e.ToString()};", false);
            }
        }
    }

    public abstract class IUIPanel<TPanel, T> : UIPanel 
        where TPanel : IUIPanel<TPanel, T>
        where T : IPanelContext
    {
        public static readonly UIPageTypeBase<TPanel, T> uiPageType = new();
        public override UIPageTypeBase UIPageType => uiPageType;
    }

    public abstract class UIPageTypeBase
    {
        public abstract string Name { get; }
        public override string ToString() => Name;
    }
    
    public class UIPageTypeBase<TPage, T> : UIPageTypeBase where TPage : IUIPanel<TPage, T> where T : IPanelContext
    {
        public readonly string name = TypeUtil.GetName<TPage>();
        public override string Name => name;
    }
}