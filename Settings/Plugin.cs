using System.Collections.Generic;
using System.IO;
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

    private void Start()
    {
        _ = ModSettings.TryCreate(this, out var modSettings);
        _modSettings = modSettings;
        Settings.Add(_modSettings);

        // Makes sure that `DataFolderPath` is loaded before everything else.
        _modSettings.LoadShared();

        // Creating these here makes sure that mods doesn't need to check if
        // these directories exist (well, anyone could erase them at runtime,
        // but that shouldn't be expected).
        _ = Directory.CreateDirectory(Paths.SharedFolderPath!);
        _ = Directory.CreateDirectory(Paths.DataFolderPath!);

        DiscoverSettings();
    }

    private void DiscoverSettings()
    {
        // TODO(Unavailable): Would it be useful to order these by dependence
        // order? If A depends on B, then B's settings would load first.
        foreach (var (guid, info) in Chainloader.PluginInfos)
        {
            if (
                info.Instance is not { } plugin
                || guid == Id
                || !ModSettings.TryCreate(plugin, out var modSettings)
            )
                continue;

            modSettings.LoadProfile();
            modSettings.LoadShared();
            Settings.Add(modSettings);
        }

        Log.Debug($"{Settings.Count} discovered settings");
    }

    internal List<string> MissingMods(int saveSlot)
    {
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

    bool IUserSettings<UserSettings>.IsCritical => false;

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
