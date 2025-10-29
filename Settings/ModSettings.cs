using System;
using System.IO;
using Silksong.Settings.Json;

namespace Silksong.Settings;

internal record ModSettings(
    string Guid,
    IProfileSettings? Profile,
    ISharedSettings? Shared,
    IUserSettings? User
)
{
    #region LoadSaveSettings

    internal void LoadProfile()
    {
        if (Profile is not null)
            LoadJson(
                ProfileSettingsPath(),
                Guid,
                Profile.ProfileSettingsType,
                Profile.OnProfileSettingsLoadUntyped
            );
    }

    internal void SaveProfile()
    {
        if (Profile is not null)
            SaveJson(
                ProfileSettingsPath(),
                Guid,
                Profile.ProfileSettingsType,
                Profile.OnProfileSettingsSaveUntyped
            );
    }

    internal void LoadShared()
    {
        if (Shared is not null)
            LoadJson(
                SharedSettingsPath(),
                Guid,
                Shared.SharedSettingsType,
                Shared.OnSharedSettingsLoadUntyped
            );
    }

    internal void SaveShared()
    {
        if (Shared is not null)
            SaveJson(
                SharedSettingsPath(),
                Guid,
                Shared.SharedSettingsType,
                Shared.OnSharedSettingsSaveUntyped
            );
    }

    internal void LoadUser(int slot)
    {
        if (User is not null)
            LoadJson(
                UserSettingsPath(slot),
                Guid,
                User.UserSettingsType,
                User.OnUserSettingsLoadUntyped
            );
    }

    internal void SaveUser(int slot)
    {
        if (User is not null)
            SaveJson(
                UserSettingsPath(slot),
                Guid,
                User.UserSettingsType,
                User.OnUserSettingsSaveUntyped
            );
    }

    #endregion

    # region Paths

    private string ProfileSettingsPath() => Path.Combine(Paths.ProfileFolderPath, $"{Guid}.json");

    private string SharedSettingsPath() =>
        Path.Combine(Paths.SharedFolderPath!, $"{Guid}.json.dat");

    private string UserSettingsPath(int slot) =>
        Path.Combine(Paths.UserSettingsPath(slot)!, $"{Guid}.json.dat");

    # endregion

    # region LoadSaveJson

    // FIXME(Unavailable): Should these be public somewhere else? Maybe also
    // with a modified API?

    private static void LoadJson(string path, string guid, Type settingsType, Action<object> onLoad)
    {
        try
        {
            if (!File.Exists(path))
                return;

            if (Utils.TryLoadJson(path, settingsType, out var obj))
            {
                onLoad(obj);
                return;
            }

            Log.Error($"Failed to load '{settingsType.Name}' settings from {guid}");

            var backupPath = path + ".bak";
            if (!File.Exists(backupPath))
                return;

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

    private static void SaveJson(string path, string guid, Type settingsType, Func<object?> onSave)
    {
        try
        {
            if (onSave() is not { } obj)
                return;

            var backupPath = path + ".bak";

            if (File.Exists(backupPath))
                File.Delete(backupPath);

            if (File.Exists(path))
                File.Move(path, backupPath);

            if (Utils.TrySaveJson(path, obj))
                return;

            Log.Error($"Failed to save '{settingsType.Name}' settings from '{guid}'");
        }
        catch (Exception e)
        {
            Log.Exception(e);
        }
    }

    #endregion
}
