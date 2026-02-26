using System.Collections;
using System.Collections.Generic;
using Duskvern;
using UnityEngine;

public class TestUIContext : IPanelContext
{
    public int index;

    public void Print()
    {
        Debug.Log($"TestUIContext Print {index}");
    }
}

public class TestUI : IUIPanel<TestUI, IPanelContext>
{
    public override E_UIPanelType PanelType => E_UIPanelType.UITest;

    protected override void OnClose()
    {
        Debug.Log("TestUI Close"); 
    }

    protected override void OnHide()
    {
        
    }

    protected override void OnOpen(IPanelContext panelContext)
    {
        Debug.Log("TestUI OnOpen"); // 输出日志，标记TestUI面板的打开
        panelContext.Print(); // 调用面板上下文的Debug方法，用于调试面板信息

    }

    protected override void OnShow()
    {
        
    }
}


