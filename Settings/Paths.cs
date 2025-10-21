using System.Diagnostics.CodeAnalysis;
using System.IO;
using UnityEngine;

namespace Silksong.Settings;

static class Paths
{
    public static string ProfileFolderPath => BepInEx.Paths.ConfigPath;

    [field: AllowNull]
    public static string SharedFolderPath => field = field is null ? SharedPathFolder() : field;

    // TODO(Unavailable): "some sensible default idk" - BadMagic 2025
    public static string DefaultUserDataPath =>
        Path.Combine(Application.persistentDataPath, "mod-data");

    static string SharedPathFolder()
    {
        var platform = Platform.Current as DesktopPlatform;
        var userId = platform?.onlineSubsystem?.UserId;
        var accId = string.IsNullOrEmpty(userId) ? "default" : userId!;
        var modSettingsPath = Path.Combine(Application.persistentDataPath, accId, "mod-settings");

        Log.Debug($"Mod settings directory found at: {modSettingsPath}");

        return modSettingsPath;
    }
}
