namespace Settings;

internal static class Log
{
    internal static void Debug(string message)
    {
        UnityEngine.Debug.Log($"[{Plugin.Id}] {message}");
    }

    internal static void Error(string message)
    {
        UnityEngine.Debug.LogError($"[{Plugin.Id}] {message}");
    }
}
