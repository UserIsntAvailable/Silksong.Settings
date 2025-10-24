using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using BepInEx;
using BepInEx.Bootstrap;
using HarmonyLib;
using JetBrains.Annotations;

namespace Silksong.Settings;

[BepInAutoPlugin(id: "unavailable.settings")]
public partial class Plugin : BaseUnityPlugin, ISharedSettings<Settings>
{
    Harmony _harmony = null!;

    internal static Plugin Instance = null!;

    internal readonly List<ModSettings> Settings = [];

    void Awake()
    {
        Log.Debug("Mod loaded");

        Instance = this;
        _harmony = new Harmony(Plugin.Id);
        _harmony.PatchAll(typeof(Patches));
    }

    [SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
    void Start()
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

            Settings.Add(new(guid, profile, shared, user));
        }

        Log.Debug($"{Settings.Count} discovered settings");
    }

    public Settings SharedSettings { get; set; } = new();
}

public record Settings
{
    [PublicAPI]
    public string DataFolderPath { get; set; } = Paths.DefaultDataFolderPath;
}
