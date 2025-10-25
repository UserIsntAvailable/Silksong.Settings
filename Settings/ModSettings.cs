using System.IO;

namespace Silksong.Settings;

internal record ModSettings(
    string Guid,
    IProfileSettings? Profile,
    ISharedSettings? Shared,
    IUserSettings? User
)
{
    internal string ProfileSettingsPath => Path.Combine(Paths.ProfileFolderPath, $"{Guid}.json");

    internal string SharedSettingsPath => Path.Combine(Paths.SharedFolderPath, $"{Guid}.json.dat");

    internal string UserSettingsPath(int slot) =>
        Path.Combine(Paths.SharedFolderPath, $"user{slot}-modded", $"{Guid}.json.dat");
}
