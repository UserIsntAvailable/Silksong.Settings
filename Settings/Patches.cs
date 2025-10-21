using System;
using System.IO;
using BepInEx;
using HarmonyLib;
using Silksong.Settings.Json;

namespace Silksong.Settings;

class Patches
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(UIManager), nameof(UIManager.Start))]
    static void LoadSharedAndProfileSettings()
    {
        Log.Debug("Loading Shared and Profile Settings");

        // TODO(Unavailable): I should probably load this plugin's config first.

        foreach (var modSettings in Plugin.Instance.Settings.Values)
        {
            var plugin = modSettings.Plugin;

            if (modSettings.ProfileSettingsInvoker is SettingsInvoker profileInvoker)
                InvokePluginLoad(modSettings.ProfileSettingsPath, plugin, profileInvoker);
            if (modSettings.SharedSettingsInvoker is SettingsInvoker sharedInvoker)
                InvokePluginLoad(modSettings.SharedSettingsPath, plugin, sharedInvoker);
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(GameManager), nameof(GameManager.OnApplicationQuit))]
    static void SaveSharedAndProfileSettings()
    {
        Directory.CreateDirectory(Paths.SharedFolderPath);

        Log.Debug("Saving Shared and Profile Settings");

        foreach (var modSettings in Plugin.Instance.Settings.Values)
        {
            var plugin = modSettings.Plugin;

            if (modSettings.ProfileSettingsInvoker is SettingsInvoker profileInvoker)
                InvokePluginSave(modSettings.ProfileSettingsPath, plugin, profileInvoker);
            if (modSettings.SharedSettingsInvoker is SettingsInvoker sharedInvoker)
                InvokePluginSave(modSettings.SharedSettingsPath, plugin, sharedInvoker);
        }
    }

    static void InvokePluginLoad(string path, BaseUnityPlugin plugin, SettingsInvoker invoker)
    {
        try
        {
            object? obj = null;
            var settingsType = invoker.SettingsType;

            if (!File.Exists(path)) return;
            if (Utils.TryLoadJson(path, settingsType, out obj))
            {
                invoker.OnSettingsLoad.Invoke(plugin, obj);
                return;
            }

            Log.Error($"Null settings passed to {plugin.Info.Metadata.GUID}");

            var backupPath = path + ".bak";
            if (!File.Exists(backupPath)) return;
            if (Utils.TryLoadJson(backupPath, settingsType, out obj))
            {
                File.Delete(path);
                File.Copy(backupPath, path);
                invoker.OnSettingsLoad.Invoke(plugin, obj);
                return;
            }

            Log.Error($"Failed to load settings from backup");
        }
        catch (Exception e)
        {
            Log.Exception(e);
        }
    }

    static void InvokePluginSave(string path, BaseUnityPlugin plugin, SettingsInvoker invoker)
    {
        try
        {
            var obj = invoker.OnSettingsSave.Invoke(plugin);
            if (obj is null) return;

            var backupPath = path + ".bak";
            if (File.Exists(backupPath)) File.Delete(backupPath);
            if (File.Exists(path)) File.Move(path, backupPath);

            if (Utils.TrySaveJson(path, obj)) return;

            Log.Error($"Failed to save settings");
        }
        catch (Exception e)
        {
            Log.Exception(e);
        }
    }
}
