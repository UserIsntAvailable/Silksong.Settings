using System;
using JetBrains.Annotations;

namespace Silksong.Settings;

[PublicAPI]
public interface IProfileSettings<T> : IProfileSettings
{
    T ProfileSettings { get; set; }

    void OnProfileSettingsLoad(T settings) => ProfileSettings = settings;

    T OnProfileSettingsSave() => ProfileSettings;

    Type IProfileSettings.ProfileSettingsType => typeof(T);

    void IProfileSettings.OnProfileSettingsLoadUntyped(object settings) =>
        OnProfileSettingsLoad((T)settings);

    // FIXME(Unavailable): Possible null reference return???
    object IProfileSettings.OnProfileSettingsSaveUntyped() => OnProfileSettingsSave()!;
}

[PublicAPI]
public interface ISharedSettings<T> : ISharedSettings
{
    T SharedSettings { get; set; }

    void OnSharedSettingsLoad(T settings) => SharedSettings = settings;

    T OnSharedSettingsSave() => SharedSettings;

    Type ISharedSettings.SharedSettingsType => typeof(T);

    void ISharedSettings.OnSharedSettingsLoadUntyped(object settings) =>
        OnSharedSettingsLoad((T)settings);

    // FIXME(Unavailable): Possible null reference return???
    object ISharedSettings.OnSharedSettingsSaveUntyped() => OnSharedSettingsSave()!;
}

[PublicAPI]
public interface IUserSettings<T> : IUserSettings
{
    // Gets set to `null` when a save slot session is finished.
    T? UserSettings { get; set; }

    void OnUserSettingsLoad(T settings) => UserSettings = settings;

    T? OnUserSettingsSave() => UserSettings;

    // Returns whether these settings are critical for the functionality of the
    // save slot; if the mod attached to these settings is not installed when a
    // new session is started, the loading of these settings would be prevented.
    bool OnFinishedSession()
    {
        UserSettings = default;
        return true;
    }

    Type IUserSettings.UserSettingsType => typeof(T);

    void IUserSettings.OnUserSettingsLoadUntyped(object settings) =>
        OnUserSettingsLoad((T)settings);

    object? IUserSettings.OnUserSettingsSaveUntyped() => OnUserSettingsSave();

    bool IUserSettings.OnFinishedSessionUntyped() => OnFinishedSession();
}

// TODO(Unavailable): These can be hidden in another namespace + [(Obsolete)] to
// prevent people from depending on these...

public interface IProfileSettings
{
    Type ProfileSettingsType { get; }

    void OnProfileSettingsLoadUntyped(object settings);

    object OnProfileSettingsSaveUntyped();
}

public interface ISharedSettings
{
    Type SharedSettingsType { get; }

    void OnSharedSettingsLoadUntyped(object settings);

    object OnSharedSettingsSaveUntyped();
}

public interface IUserSettings
{
    Type UserSettingsType { get; }

    void OnUserSettingsLoadUntyped(object settings);

    object? OnUserSettingsSaveUntyped();

    bool OnFinishedSessionUntyped();
}
