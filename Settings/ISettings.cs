namespace Silksong.Settings;

// TODO(Unavailable): type erased interfaces

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
    T UserSettings { get; set; }

    /// Prevent from loading this user settings if the mod is not installed.
    bool Critical { get => true; }

    void OnUserSettingsLoad(T settings)
    {
        UserSettings = settings;
    }

    T OnUserSettingsSave()
    {
        return UserSettings;
    }
}
