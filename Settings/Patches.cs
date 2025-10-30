using System;
using System.IO;
using HarmonyLib;
using UnityEngine.UI;

namespace Silksong.Settings;

internal static class Patches
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(GameManager), nameof(GameManager.OnApplicationQuit))]
    private static void SaveSharedAndProfileSettings()
    {
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
        if (saveSlot == 0)
            return;

        Log.Debug($"Loading User Settings for Save Slot {saveSlot}");

        foreach (var modSettings in Plugin.Instance.Settings)
            modSettings.LoadUser(saveSlot);
    }

    [HarmonyPrefix]
    [HarmonyPatch(
        typeof(GameManager),
        nameof(GameManager.SaveGame),
        [typeof(int), typeof(Action<bool>), typeof(bool), typeof(AutoSaveName)]
    )]
    // TODO(Unavailable): Implement "Restore_Points" functionality.
    private static void SaveUserSettings(int saveSlot, ref Action<bool> ogCallback)
    {
        if (saveSlot == 0)
            return;

        var ogCallbackCopy = ogCallback;
        ogCallback = (didSave) =>
        {
            ogCallbackCopy?.Invoke(didSave);

            // FIXME(Unavailable): I think it is a good idea to not save any
            // settings if saving the original save slot data fails, but I'm
            // not really sure about it.
            if (!didSave)
                return;

            var settings = Plugin.Instance.Settings;

            foreach (var modSettings in settings)
            {
                if (modSettings.User is not { } user)
                    continue;

                var userSettings = Plugin.Instance.UserSettings ??= new();
                userSettings.CriticalUserSettings[modSettings.Guid] = user.IsCriticalUntyped;
            }

            _ = Directory.CreateDirectory(Paths.UserSettingsPath(saveSlot)!);

            Log.Debug($"Saving User Settings for Save Slot {saveSlot}");

            foreach (var modSettings in settings)
                modSettings.SaveUser(saveSlot);
        };
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(GameManager), nameof(GameManager.ReturnToMainMenu))]
    private static void SetModUserSettingsToNull(ref Action<bool> callback)
    {
        var callbackCopy = callback;
        callback = (saveCompleteValue) =>
        {
            callbackCopy?.Invoke(saveCompleteValue);

            Log.Debug("Returning to main menu. Zeroing out all User Settings");
            foreach (var modSettings in Plugin.Instance.Settings)
                if (modSettings.User is { } user)
                    user.UserSettingsUntyped = null;
        };
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(GameManager), nameof(GameManager.ClearSaveFile))]
    private static void ClearUserSettings(int saveSlot)
    {
        if (saveSlot == 0)
            return;

        var userSettingsFolder = Paths.UserSettingsPath(saveSlot)!;
        if (!Directory.Exists(userSettingsFolder))
            return;

        Log.Debug($"Clearing User Settings for Save Slot {saveSlot}");
        Directory.Delete(userSettingsFolder, true);
    }
}
