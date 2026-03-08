using UnityEngine;

public static class SimpleSaveData 
{
    public static string test
    {
        get
        {
            return PlayerPrefs.GetString("test");
        }
        set
        {
            PlayerPrefs.SetString("test", value);
        }
    }
}
