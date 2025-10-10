using BepInEx;
using HarmonyLib;

namespace Settings;

[BepInAutoPlugin(id: "unavailable.settings")]
public partial class Plugin : BaseUnityPlugin
{
    static Harmony _harmony = null!;

    void Awake()
    {
        Log.Debug("Mod loaded");

        _harmony = new Harmony(Plugin.Id);
        _harmony.PatchAll(typeof(Patch));
    }

    void OnDestroy()
    {
        _harmony.UnpatchSelf();
        Log.Debug("Mod unloaded");
    }
}

class Patch { }
