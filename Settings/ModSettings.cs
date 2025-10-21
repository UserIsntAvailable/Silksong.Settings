using System;
using System.IO;
using System.Reflection;
using BepInEx;

namespace Silksong.Settings;

record ModSettings(
    BaseUnityPlugin Plugin,
    SettingsInvoker? ProfileSettingsInvoker,
    SettingsInvoker? SharedSettingsInvoker,
    SettingsInvoker? UserSettingsInvoker
)
{
    string Filename => $"{this.Plugin.Info.Metadata.GUID}.json";

    internal string ProfileSettingsPath => Path.Combine(Paths.ProfileFolderPath, Filename);
    internal string SharedSettingsPath => Path.Combine(Paths.SharedFolderPath, Filename + ".dat");

    internal string UserSettingsPath(int slot) =>
        Path.Combine(Paths.SharedFolderPath, $"user{slot}-modded", Filename + ".dat");
}

record SettingsInvoker(
    Type SettingsType,
    ReflectionMethod OnSettingsLoad,
    ReflectionMethod OnSettingsSave
);

record ReflectionMethod
{
    // PERF(Unavailable): Use something akin to monomod's `FastReflectionDelegate`
    // (on `reorg` this is called `FastInvoker`).
    MethodInfo Info { get; init; }

    ReflectionMethod(MethodInfo info)
    {
        Info = info;
    }

    internal object? Invoke(object obj, params object[] parameters) => Info.Invoke(obj, parameters);

    public static implicit operator ReflectionMethod(MethodInfo info) => new(info);
}
