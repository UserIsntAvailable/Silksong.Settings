using System.IO;
using HarmonyLib;

namespace Silksong.Settings;

internal static class Patches
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(UIManager), nameof(UIManager.Start))]
    private static void LoadSharedAndProfileSettings()
    {
        Log.Debug("Loading Shared and Profile Settings");

        foreach (var modSettings in Plugin.Instance.Settings)
        {
            modSettings.LoadProfile();
            modSettings.LoadShared();
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(GameManager), nameof(GameManager.OnApplicationQuit))]
    private static void SaveSharedAndProfileSettings()
    {
        _ = Directory.CreateDirectory(Paths.SharedFolderPath!);

        Log.Debug("Saving Shared and Profile Settings");

        foreach (var modSettings in Plugin.Instance.Settings)
        {
            modSettings.SaveProfile();
            modSettings.SaveShared();
        }
    }
}
