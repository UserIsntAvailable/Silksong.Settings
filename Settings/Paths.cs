using System.IO;
using JetBrains.Annotations;
using UnityEngine;

namespace Silksong.Settings;

public static class Paths
{
    public static string ProfileFolderPath => BepInEx.Paths.ConfigPath;

    // DOCS(Unavailable): Explain why it can be null (read `ModSettingsPath()`
    // for context.
    public static string? SharedFolderPath =>
        Plugin.Instance.didStart ? field ??= ModSettingsPath() : null;

    // DOCS(Unavailable): Explain why it can be null (read `ModSettingsPath()`
    // for context.
    public static string? UserSettingsPath(int slot) =>
        SharedFolderPath is null
            ? null
            : Path.Combine(SharedFolderPath, $"user{slot}-modded");

    // DOCS(Unavailable): Explain why it can be null (read `ModSettingsPath()`
    // for context.
    [PublicAPI]
    public static string? DataFolderPath =>
        Plugin.Instance.didStart ? Plugin.Instance.SharedSettings.DataFolderPath : null;

    // FIXME(Unavailable): "some sensible default idk" - BadMagic 2025
    internal static string DefaultDataFolderPath =>
        Path.Combine(Application.persistentDataPath, "mod-data");

    // FIXME(Unavailable):
    //
    // This shouldn't be called in `Awake()`, since `SteamOnlineSubsystem` would
    // have not run yet, which would make `saveDirPath` point to the "default"
    // folder path. Other than returning `null` for the `*FolderPath` methods
    // these are the possible solutions (or mix of them):
    //
    // 1. `throw` instead of `?`.
    //
    // 2. Change the modding template to use `Start()` over `Awake()`, since in
    // theory it doesn't really matter which one is used.
    //
    // 3. Preload/Patch `SteamOnlineSubsystem` so that it runs before `BepInEx`
    // Chainloader.
    //
    // 4. Modding Analyzer.
    private static string ModSettingsPath()
    {
        var platform = Platform.Current as DesktopPlatform;
        var modSettingsPath = Path.Combine(platform!.saveDirPath, "mod-settings");

        Log.Debug($"Mod settings directory found at: {modSettingsPath}");

        return modSettingsPath;
    }
}
