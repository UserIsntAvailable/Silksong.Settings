using System;
using System.IO;
using HarmonyLib;
using Silksong.Settings.Json;

namespace Silksong.Settings;

static class Patches
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(UIManager), nameof(UIManager.Start))]
    static void LoadSharedAndProfileSettings()
    {
        Log.Debug("Loading Shared and Profile Settings");

        foreach (var modSettings in Plugin.Instance.Settings)
        {
            var guid = modSettings.Guid;

            if (modSettings.Profile is { } profile)
                LoadModSettings(
                    modSettings.ProfileSettingsPath,
                    guid,
                    profile.ProfileSettingsType,
                    profile.OnProfileSettingsLoadUntyped
                );

            if (modSettings.Shared is { } shared)
                LoadModSettings(
                    modSettings.SharedSettingsPath,
                    guid,
                    shared.SharedSettingsType,
                    shared.OnSharedSettingsLoadUntyped
                );
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(GameManager), nameof(GameManager.OnApplicationQuit))]
    static void SaveSharedAndProfileSettings()
    {
        Directory.CreateDirectory(Paths.SharedFolderPath);

        Log.Debug("Saving Shared and Profile Settings");

        foreach (var modSettings in Plugin.Instance.Settings)
        {
            var guid = modSettings.Guid;

            if (modSettings.Profile is { } profile)
                SaveModSettings(
                    modSettings.ProfileSettingsPath,
                    guid,
                    profile.ProfileSettingsType,
                    profile.OnProfileSettingsSaveUntyped
                );

            if (modSettings.Shared is { } shared)
                SaveModSettings(
                    modSettings.SharedSettingsPath,
                    guid,
                    shared.SharedSettingsType,
                    shared.OnSharedSettingsSaveUntyped
                );
        }
    }

    internal static void LoadModSettings(
        string path,
        string guid,
        Type settingsType,
        Action<object> onLoad
    )
    {
        try
        {
            if (!File.Exists(path)) return;
            if (Utils.TryLoadJson(path, settingsType, out var obj))
            {
                onLoad(obj);
                return;
            }

            Log.Error($"Failed to load '{settingsType.Name}' settings from {guid}");

            var backupPath = path + ".bak";
            if (!File.Exists(backupPath)) return;
            if (Utils.TryLoadJson(backupPath, settingsType, out obj))
            {
                File.Delete(path);
                File.Copy(backupPath, path);
                onLoad(obj);
                return;
            }

            Log.Error($"Failed to load '{settingsType.Name}' settings from backup");
        }
        catch (Exception e)
        {
            Log.Exception(e);
        }
    }

    internal static void SaveModSettings(
        string path,
        string guid,
        Type settingsType,
        Func<object?> onSave
    )
    {
        try
        {
            if (onSave() is not { } obj) return;

            var backupPath = path + ".bak";
            if (File.Exists(backupPath)) File.Delete(backupPath);
            if (File.Exists(path)) File.Move(path, backupPath);
            if (Utils.TrySaveJson(path, obj)) return;

            Log.Error($"Failed to save '{settingsType.Name}' settings from '{guid}'");
        }
        catch (Exception e)
        {
            Log.Exception(e);
        }
    }
}
