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

    // Code modified from `dpinela/DataManager`. Copyright notice:
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

        if (!Platform.IsSaveSlotIndexValid(saveSlot))
            return false;

        var missing = Plugin.Instance.CriticalUserSettingsStatsForSlot(saveSlot);

        if (missing.Count == 0)
            return true;

        Log.Debug($"save slot {saveSlot} has save data for missing mods:");
        foreach (var m in missing)
            Log.Debug(m);

        // to match the behavior of the original method
        CheatManager.LastErrorText = errorInfo;
        __instance.ChangeSaveFileState(SaveSlotButton.SaveFileStates.Incompatible, doAnimate);
        __result = true;

        return false;
    }

    // Code modified from `dpinela/DataManager`. Copyright notice:
    //
    // Licensed under the EUPL-1.2
    // Copyright (c) 2025 silksong-modding
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
            if (didSave)
            {
                Log.Debug($"Saving User Settings for Save Slot '{saveSlot}'");
                foreach (var modSettings in Plugin.Instance.Settings)
                    modSettings.SaveUser(saveSlot);
            }
            ogCallbackCopy(didSave);
        };
    }

    // TODO(Unavailable): This should happen on `GameManager.ReturnToMainMenu`
    private static void SetModUserSettingsToNull() { }
}
