namespace Silksong.Settings;

// TODO(Unavailable): type erased interfaces?

public interface IProfileSettings<T>
{
    T ProfileSettings { get; set; }

    void OnProfileSettingsLoad(T settings)
    {
        ProfileSettings = settings;
    }

    T OnProfileSettingsSave()
    {
        return ProfileSettings;
    }
}

public interface ISharedSettings<T>
{
    T SharedSettings { get; set; }

    void OnSharedSettingsLoad(T settings)
    {
        SharedSettings = settings;
    }

    T OnSharedSettingsSave()
    {
        return SharedSettings;
    }
}

public interface IUserSettings<T>
{
    // Gets set to `null` when a save slot session is finished (notified by
    // `OnFinishedSession`).
    T? UserSettings { get; set; }

    void OnUserSettingsLoad(T settings)
    {
        UserSettings = settings;
    }

    T? OnUserSettingsSave()
    {
        return UserSettings;
    }

    // Returns wether or not these settings are critical for the functionality
    // of the save slot; if the mod attached to these settings is not installed
    // when a new session is started, the loading of these settings would be
    // prevented.
    bool OnFinishedSession() => true;
}
