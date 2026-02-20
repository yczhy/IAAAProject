using UnityEngine;
using Cysharp.Text; // 引入 ZString

public static class StringUtil
{
    public static string GetString(string str1)
    {
        return str1 ?? string.Empty;
    }
    
    public static string GetString(string str1, string str2)
    {
        return ZString.Concat(str1 ?? string.Empty, str2 ?? string.Empty);
    }
    
    public static string GetString(string str1, string str2, string str3)
    {
        return ZString.Concat(
            str1 ?? string.Empty,
            str2 ?? string.Empty,
            str3 ?? string.Empty
        );
    }
    
    public static string GetString(string str1, string str2, string str3, string str4)
    {
        return ZString.Concat(
            str1 ?? string.Empty,
            str2 ?? string.Empty,
            str3 ?? string.Empty,
            str4 ?? string.Empty
        );
    }
    
    public static string GetString(string str1, string str2, string str3, string str4, string str5)
    {
        return ZString.Concat(
            str1 ?? string.Empty,
            str2 ?? string.Empty,
            str3 ?? string.Empty,
            str4 ?? string.Empty,
            str5 ?? string.Empty
        );
    }
    
    public static string GetString(string str1, string str2, string str3, string str4, string str5, string str6)
    {
        return ZString.Concat(
            str1 ?? string.Empty,
            str2 ?? string.Empty,
            str3 ?? string.Empty,
            str4 ?? string.Empty,
            str5 ?? string.Empty,
            str6 ?? string.Empty
        );
    }
}