using System;

namespace Silksong.Settings;

internal static class Log
{
    internal static void Debug(string message)
    {
        UnityEngine.Debug.Log($"[{Plugin.Name}] {message}");
    }

    internal static void Error(string message)
    {
        UnityEngine.Debug.LogError($"[{Plugin.Name}] {message}");
    }

    internal static void Exception(Exception e)
    {
        UnityEngine.Debug.LogError($"[{Plugin.Name}] {e}");
    }
}
