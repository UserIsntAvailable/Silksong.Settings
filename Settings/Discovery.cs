using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Bootstrap;

namespace Silksong.Settings;

internal static class Discovery
{
    internal static Dictionary<string, ModSettings> FindModSettings()
    {
        Dictionary<string, ModSettings> settings = new();

        foreach (var plugin in Chainloader.PluginInfos.Values)
        {
            if (plugin is null) continue;
            var interfaces = plugin.Instance.GetType().GetInterfaces();

            var profileInvoker = ImplementsSettings(
                interfaces,
                typeof(IProfileSettings<>),
                nameof(IProfileSettings<>.OnProfileSettingsLoad),
                nameof(IProfileSettings<>.OnProfileSettingsSave)
            );

            var sharedInvoker = ImplementsSettings(
                interfaces,
                typeof(ISharedSettings<>),
                nameof(ISharedSettings<>.OnSharedSettingsLoad),
                nameof(ISharedSettings<>.OnSharedSettingsSave)
            );

            var userInvoker = ImplementsSettings(
                interfaces,
                typeof(IUserSettings<>),
                nameof(IUserSettings<>.OnUserSettingsLoad),
                nameof(IUserSettings<>.OnUserSettingsSave)
            );

            if (profileInvoker is null && sharedInvoker is null && userInvoker is null)
                continue;

            settings[plugin.Metadata.GUID] = new(
                plugin.Instance,
                profileInvoker,
                sharedInvoker,
                userInvoker
            );
        }

        return settings;
    }

    static SettingsInvoker? ImplementsSettings(
        Type[] interfaces,
        Type settingsType,
        string onLoadMethod,
        string onSaveMethod
    )
    {
        SettingsInvoker? invoker = null;
        if (
            interfaces.FirstOrDefault(x =>
                x.IsGenericType && x.GetGenericTypeDefinition() == settingsType
            )
            is { } iSettings
        )
        {
            invoker = new(
                iSettings.GetGenericArguments()[0],
                iSettings.GetMethod(onLoadMethod),
                iSettings.GetMethod(onSaveMethod)
            );
        }
        return invoker;
    }
}
