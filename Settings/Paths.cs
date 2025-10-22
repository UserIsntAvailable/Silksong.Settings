using System.IO;
using UnityEngine;

namespace Silksong.Settings;

public static class Paths
{
    public static string ProfileFolderPath => BepInEx.Paths.ConfigPath;

    // DOCS(Unavailable): Can be null if: 1) gets called inside `Awake()` or
    // 2) dev didn't set `Silksong.Settings` as one of their dependencies, and
    // their `Start()` method ran first that ours.
    public static string? SharedFolderPath
    {
        get
        {
            // TODO(Unavailable): Wouldn't it be better to `throw`?
            if (!Plugin.Instance.didStart) return null;
            return field ??= ModSettingsFolderPath();
        }
    }

    // TODO(Unavailable): Special case the loading of myself to happen in
    // `Start()`; like that, mods can easily get `DataFolderPath` even
    // before getting to `UIManager.Start`.
    //
    // public static string? DataFolderPath
    //    => Plugin.Instance.ProfileSettings.DataFolderPath

    // TODO(Unavailable): "some sensible default idk" - BadMagic 2025
    internal static string DefaultDataFolderPath =>
        Path.Combine(Application.persistentDataPath, "mod-data");

    static string ModSettingsFolderPath()
    {
        var platform = Platform.Current as DesktopPlatform;
        var userId = platform?.onlineSubsystem?.UserId;
        var accId = string.IsNullOrEmpty(userId) ? "default" : userId!;
        var modSettingsPath = Path.Combine(Application.persistentDataPath, accId, "mod-settings");

        Log.Debug($"Mod settings directory found at: {modSettingsPath}");

        return modSettingsPath;
    }
}
