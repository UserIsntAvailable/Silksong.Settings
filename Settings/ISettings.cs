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

    Type IUserSettings.UserSettingsType => typeof(T);

    object? IUserSettings.UserSettingsUntyped
    {
        get => UserSettings;
        set => UserSettings = value == null ? default : (T)value;
    }

    void IUserSettings.OnUserSettingsLoadUntyped(object settings) =>
        OnUserSettingsLoad((T)settings);

    object? IUserSettings.OnUserSettingsSaveUntyped() => OnUserSettingsSave();
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

    object? UserSettingsUntyped { get; set; }

    void OnUserSettingsLoadUntyped(object settings);

    object? OnUserSettingsSaveUntyped();
}
