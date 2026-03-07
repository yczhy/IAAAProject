using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System.IO;
using System;

public class ImportDataForLuban 
{
    [MenuItem("Tools/Luban/执行导表脚本 (gen.sh) %&g", false, 10)]
    public static void RunGenScript()
    {
        // 1. 定位路径逻辑
        string assetsPath = Application.dataPath;
        if (!Directory.Exists(assetsPath)) {
            UnityEngine.Debug.LogError("[Luban] 无法找到 Assets 目录");
            return;
        }

        string gameProjectRoot = Directory.GetParent(assetsPath).FullName;
        string iaaaProjectRoot = Directory.GetParent(gameProjectRoot).FullName;
        string miniTemplateDir = Path.Combine(iaaaProjectRoot, "MiniTemplate");
        
        // 查找脚本
        string scriptPath = Path.Combine(miniTemplateDir, "gen.sh");
        if (!File.Exists(scriptPath)) {
            scriptPath = Path.Combine(miniTemplateDir, "gen.command");
        }

        if (!File.Exists(scriptPath)) {
            UnityEngine.Debug.LogError($"[Luban] 错误：找不到脚本文件。\n检查路径: {miniTemplateDir}");
            EditorUtility.DisplayDialog("错误", $"未找到 gen.sh 或 gen.command\n在:\n{miniTemplateDir}", "确定");
            return;
        }

        UnityEngine.Debug.Log($"[Luban] 准备执行脚本: {scriptPath}");

        // 2. 【关键步骤】确保脚本有执行权限 (兼容所有 Unity 版本)
        SetExecutablePermission(scriptPath);

        // 3. 构建进程
        // 直接调用 /bin/bash 并传入脚本路径
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = "/bin/bash", 
            Arguments = $"\"{scriptPath}\"", 
            WorkingDirectory = miniTemplateDir, 
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
            StandardOutputEncoding = System.Text.Encoding.UTF8,
            StandardErrorEncoding = System.Text.Encoding.UTF8
        };

        try
        {
            using (Process process = new Process { StartInfo = startInfo })
            {
                process.OutputDataReceived += (sender, e) => { 
                    if (!string.IsNullOrEmpty(e.Data)) UnityEngine.Debug.Log("[Luban-Script] " + e.Data); 
                };
                process.ErrorDataReceived += (sender, e) => { 
                    if (!string.IsNullOrEmpty(e.Data)) UnityEngine.Debug.LogError("[Luban-Script] " + e.Data); 
                };

                UnityEngine.Debug.Log($"[Luban] 启动命令: {startInfo.FileName} {startInfo.Arguments}");
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();

                if (process.ExitCode == 0)
                {
                    UnityEngine.Debug.Log("[Luban] ✅ 脚本执行成功！");
                    EditorUtility.DisplayDialog("成功", "Luban 导表完成！", "好的");
                    AssetDatabase.Refresh();
                }
                else
                {
                    UnityEngine.Debug.LogError($"[Luban] ❌ 脚本执行失败，退出码: {process.ExitCode}");
                    EditorUtility.DisplayDialog("失败", $"执行失败 (Exit Code: {process.ExitCode})\n请查看 Console 详细日志。", "确定");
                }
            }
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogError($"[Luban] 启动进程异常: {ex.Message}\n{ex.StackTrace}");
            EditorUtility.DisplayDialog("异常", $"无法启动脚本:\n{ex.Message}", "确定");
        }
    }

    /// <summary>
    /// 使用 'chmod +x' 命令赋予脚本执行权限
    /// 此方法兼容所有 Unity/.NET 版本
    /// </summary>
    private static void SetExecutablePermission(string filePath)
    {
        // 仅在 Mac 或 Linux 上需要
        if (Application.platform != RuntimePlatform.OSXEditor && 
            Application.platform != RuntimePlatform.LinuxEditor)
        {
            return;
        }

        try 
        {
            ProcessStartInfo chmodInfo = new ProcessStartInfo
            {
                FileName = "/bin/chmod",
                Arguments = $"+x \"{filePath}\"",
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            using (Process chmodProc = new Process { StartInfo = chmodInfo })
            {
                chmodProc.Start();
                chmodProc.WaitForExit(2000); // 最多等待2秒
                
                if (chmodProc.ExitCode == 0)
                {
                    UnityEngine.Debug.Log($"[Luban] 已成功赋予执行权限: {filePath}");
                }
                else
                {
                    UnityEngine.Debug.LogWarning($"[Luban] chmod 命令返回非零退出码: {chmodProc.ExitCode}，但将继续尝试运行。");
                }
            }
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogWarning($"[Luban] 设置权限时发生异常: {ex.Message}。如果脚本报错 'Permission denied'，请手动在终端运行 'chmod +x {filePath}'。");
        }
    }
}