using System.Collections;
using System.Collections.Generic;
using System.IO;
using SimpleJSON;
using UnityEngine;

public static class ConfigManager 
{
    public static cfg.Tables Tables;

    public static void Load()
    {
        string configPath = Path.Combine(Application.streamingAssetsPath, "Config");

        Tables = new cfg.Tables(file =>
        {
            string path = Path.Combine(configPath, file + ".json");
            string json = File.ReadAllText(path);
            return JSON.Parse(json);
        });

        Debug.Log("Config Load Complete");
    }
}
