using System.ComponentModel;
using Duskvern;
using SRDebugger.Internal;

public partial class SROptions
{
    [Category("SaveData"), DisplayName("加载存档")]
    public void LoadSaveData()
    {
        var timeSaveData = SaveDataModule.Instance.GetData<TimeSaveData>();
        DebugLogger.LogInfo($"name " + timeSaveData.name, false);
    }

    [Category("SaveData"), DisplayName("保存存档")]
    public void SaveData()
    {
        var timeSaveData = SaveDataModule.Instance.GetData<TimeSaveData>();
        timeSaveData.name = "修改名字";
        DebugLogger.LogInfo($"name " + timeSaveData.name, false);
    }

    private int index = 0;
    [Category("SaveData"), DisplayName("修改存档")]
    public void AlterSaveData()
    {
        var timeSaveData = SaveDataModule.Instance.GetData<TimeSaveData>();
        timeSaveData.name = "修改名字 " + index++;
        DebugLogger.LogInfo($"修改后名字为 " + timeSaveData.name, false);
    }
}
