using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Text; // 引入 ZString 命名空间

namespace Duskvern
{
    public static class DebugLogger
    {
        public static bool EnableLogs = true;

        public static void LogInfo(string tag, string content = "", bool editorOnly = false)
        {
            if (editorOnly && !Application.isEditor) return;
            Log(LogType.Info, tag, content);
        }

        public static void LogWarning(string tag, string content = "", bool editorOnly = false)
        {
            if (editorOnly && !Application.isEditor) return;
            Log(LogType.Warning, tag, content);
        }

        public static void LogError(string tag, string content = "", bool editorOnly = false)
        {
            if (editorOnly && !Application.isEditor) return;
            Log(LogType.Error, tag, content);
        }

        public static void LogInfo(string content = "", bool editorOnly = false)
        {
            if (editorOnly && !Application.isEditor) return;
            Log(LogType.Info, "", content);
        }

        public static void LogWarning(string content = "", bool editorOnly = false)
        {
            if (editorOnly && !Application.isEditor) return;
            Log(LogType.Warning, "", content);
        }

        public static void LogError(string content = "", bool editorOnly = false)
        {
            if (editorOnly && !Application.isEditor) return;
            Log(LogType.Error, "", content);
        }

        private static void Log(LogType logType, string tag, string content = "")
        {
            if (!EnableLogs) return;

            // 使用 ZString 高性能拼接
            string message;
            if (string.IsNullOrEmpty(tag))
            {
                message = content;
            }
            else
            {
                message = ZString.Concat("[", tag, "]: ", content);
            }

            switch (logType)
            {
                case LogType.Info:
                    Debug.Log(message);
                    break;
                case LogType.Warning:
                    Debug.LogWarning(message);
                    break;
                case LogType.Error:
                    Debug.LogError(message);
                    break;
            }
        }

        private enum LogType
        {
            Info,
            Warning,
            Error
        }
    }
}