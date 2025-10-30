using System;
using System.IO;
using HarmonyLib;
using UnityEngine.UI;

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

    // Copyright notice from `dpinela/DataManager`:
    //
    // Licensed under the EUPL-1.2
    // Copyright (c) 2025 silksong-modding
    [HarmonyPrefix]
    [HarmonyPatch(typeof(SaveSlotButton), nameof(SaveSlotButton.ProcessSaveStats))]
    private static bool CheckCriticalUserSettingsStats(
        SaveSlotButton __instance,
        ref bool __result,
        bool doAnimate,
        string errorInfo
    )
    {
        var saveSlot = __instance.SaveSlotIndex;
        var missingMods = Plugin.Instance.MissingMods(saveSlot);
        if (missingMods.Count == 0)
        {
            return true;
        }
        Log.Debug($"save slot {saveSlot} has save data for missing mods:");
        foreach (var m in missingMods)
        {
            Log.Debug(m);
        }
        // to match the behavior of the original method
        CheatManager.LastErrorText = errorInfo;
        __instance.ChangeSaveFileState(SaveSlotButton.SaveFileStates.Incompatible, doAnimate);
        __result = true;
        return false;
    }

    [HarmonyPostfix]
    [HarmonyPatch(
        typeof(GameManager),
        nameof(GameManager.SetLoadedGameData),
        [typeof(SaveGameData), typeof(int)]
    )]
    private static void LoadUserSettings(int saveSlot)
    {
        if (!Platform.IsSaveSlotIndexValid(saveSlot))
            return;

        _ = Directory.CreateDirectory(Paths.UserSettingsPath(saveSlot)!);

        Log.Debug($"Loading User Settings for Save Slot '{saveSlot}'");

        foreach (var modSettings in Plugin.Instance.Settings)
            modSettings.LoadUser(saveSlot);
    }

    [HarmonyPrefix]
    [HarmonyPatch(
        typeof(GameManager),
        nameof(GameManager.SaveGame),
        [typeof(int), typeof(Action<bool>), typeof(bool), typeof(AutoSaveName)]
    )]
    private static void SaveUserSettings(int saveSlot, ref Action<bool> ogCallback)
    {
        var ogCallbackCopy = ogCallback;
        ogCallback = (didSave) =>
        {
            ogCallbackCopy(didSave);

            if (didSave)
            {
                Log.Debug($"Saving User Settings for Save Slot '{saveSlot}'");

                var settings = Plugin.Instance.Settings;

                foreach (var modSettings in settings)
                {
                    if (modSettings.User is { } user && user.IsCriticalUntyped)
                    {
                        var userSettings = Plugin.Instance.UserSettings ??= new();
                        userSettings.CriticalUserSettings[modSettings.Guid] = true;
                    }
                }

                foreach (var modSettings in settings)
                    modSettings.SaveUser(saveSlot);
            }
        };
    }

    // TODO(Unavailable): This should happen on `GameManager.ReturnToMainMenu`
    private static void SetModUserSettingsToNull() { }
}
