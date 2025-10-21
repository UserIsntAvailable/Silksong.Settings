using System.Collections.Generic;
using BepInEx;
using HarmonyLib;

namespace Silksong.Settings;

[BepInAutoPlugin(id: "unavailable.settings")]
public partial class Plugin : BaseUnityPlugin, IProfileSettings<Settings>
{
    Harmony _harmony = null!;

    internal static Plugin Instance = null!;

    // TODO(Unavailable): This doesn't need to be a `Dictionary` anymore.
    internal Dictionary<string, ModSettings> Settings = new();

    void Awake()
    {
        Log.Debug("Mod loaded");

        Instance = this;
        _harmony = new Harmony(Plugin.Id);
        _harmony.PatchAll(typeof(Patches));
    }

    void Start()
    {
        Settings = Discovery.FindModSettings();
        Log.Debug($"{Settings.Keys.Count} discovered settings");
    }

    // TODO(Unavailable): This should be `ISharedSettings`
    public Settings ProfileSettings { get; set; } = new();
}

public record Settings
{
    public string DataFolderPath { get; set; } = Paths.DefaultDataFolderPath;
}
