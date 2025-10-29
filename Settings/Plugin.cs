using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using BepInEx;
using BepInEx.Bootstrap;
using HarmonyLib;
using JetBrains.Annotations;

namespace Silksong.Settings;

[BepInAutoPlugin(id: "unavailable.settings")]
public partial class Plugin : BaseUnityPlugin
{
    private Harmony _harmony = null!;
    private ModSettings _modSettings = null!;

    internal static Plugin Instance { get; private set; } = null!;

    internal readonly List<ModSettings> Settings = [];

    private void Awake()
    {
        Log.Debug("Mod loaded");

        Instance = this;
        _harmony = new Harmony(Id);
        _harmony.PatchAll(typeof(Patches));
    }

    [SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
    private void Start()
    {
        foreach (var (guid, info) in Chainloader.PluginInfos)
        {
            if (info.Instance is not { } plugin)
                continue;

            var profile = plugin as IProfileSettings;
            var shared = plugin as ISharedSettings;
            var user = plugin as IUserSettings;

            if (profile is null && shared is null && user is null)
                continue;

            ModSettings modSettings = new(guid, profile, shared, user);
            if (guid == Id)
            {
                _modSettings = modSettings;
                // Shared settings are specially handled to load `DataFolderPath`
                // before `Patches.LoadSharedAndProfileSettings`.
                modSettings = modSettings with
                {
                    Shared = null,
                };
            }

            Settings.Add(modSettings);
        }

        Log.Debug($"{Settings.Count} discovered settings");

        _modSettings.LoadShared();
    }

    private void OnDestroy() => _modSettings.SaveShared();

    internal List<string> CriticalUserSettingsStatsForSlot(int saveSlot)
    {
        // TODO(Unavailable): I think `Json/Utils` should be public in order
        // to make it easier for people reading/writing files when using `Paths`.
        _modSettings.LoadUser(saveSlot);
        if (UserSettings?.CriticalUserSettings is not { } settings)
            return [];

        var missing = settings
            .Where(x => x.Value && !Chainloader.PluginInfos.ContainsKey(x.Key))
            .Select(x => x.Key)
            .ToList();
        UserSettings = null;
        return missing;
    }
}

// Plugin's Settings

public partial class Plugin : ISharedSettings<SharedSettings>, IUserSettings<UserSettings>
{
    public SharedSettings SharedSettings { get; set; } = new();

    public UserSettings? UserSettings { get; set; }

    void ISharedSettings<SharedSettings>.OnSharedSettingsLoad(SharedSettings settings)
    {
        Log.Debug($"Data directory found at: {settings.DataFolderPath}");
        SharedSettings = settings;
    }
}

public record SharedSettings
{
    [PublicAPI]
    public string DataFolderPath { get; set; } = Paths.DefaultDataFolderPath;
}

public record UserSettings
{
    [PublicAPI]
    public Dictionary<string, bool> CriticalUserSettings { get; set; } = [];
}
