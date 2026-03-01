using Duskvern;
using SRDebugger.Internal;

public partial class SROptions
{
    private int _testUiOpenIndex;

    [System.ComponentModel.Category("UI"), DisplayName("打开TestUI")]
    public async void OpenTestUI()
    {
        var context = new TestUIContext
        {
            index = ++_testUiOpenIndex
        };

        await UIModule.Instance.OpenPage(TestUI.uiPageType, context);
    }

    [System.ComponentModel.Category("UI"), DisplayName("关闭TestUI")]
    public async void CloseTestUI()
    {
        var panel = UIModule.Instance.GetPage<TestUI>();
        if (panel == null)
        {
            return;
        }
        
        await UIModule.Instance.ClosePanel(panel);
    }
}
