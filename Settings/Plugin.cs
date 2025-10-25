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

        Patches.LoadModSettings(
            _modSettings.SharedSettingsPath,
            Id,
            _modSettings.Shared!.SharedSettingsType,
            _modSettings.Shared!.OnSharedSettingsLoadUntyped
        );
    }

    private void OnDestroy()
    {
        Patches.SaveModSettings(
            _modSettings.SharedSettingsPath,
            Id,
            _modSettings.Shared!.SharedSettingsType,
            _modSettings.Shared!.OnSharedSettingsSaveUntyped
        );
    }

    public Settings SharedSettings { get; set; } = new();
}

public record Settings
{
    [PublicAPI]
    public string DataFolderPath { get; set; } = Paths.DefaultDataFolderPath;
}
