using System;
using System.IO;
using Newtonsoft.Json;

namespace Settings.Json;

static class Utils
{
    static JsonSerializerSettings _settings = new()
    {
        ContractResolver = ShouldSerializeContractResolver.Instance,
        TypeNameHandling = TypeNameHandling.Auto,
        ObjectCreationHandling = ObjectCreationHandling.Replace,
        // TODO(Unavailable): Bring back `JsonConverterTypes`?
        Converters = [new Vector2Converter(), new Vector3Converter()],
    };

    internal static bool TryLoadJson(string path, Type type, out object value)
    {
        try
        {
            var json = File.ReadAllText(path);
            var obj = JsonConvert.DeserializeObject(json, type, _settings);
            if (obj is not null)
            {
                value = obj;
                return true;
            }
        }
        catch (Exception e)
        {
            Log.Exception(e);
        }

        value = null!;
        return false;
    }

    internal static bool TrySaveJson(string path, object value)
    {
        try
        {
            File.WriteAllText(
                path,
                JsonConvert.SerializeObject(value, Formatting.Indented, _settings)
            );
            return true;
        }
        catch (Exception e)
        {
            Log.Exception(e);
            return false;
        }
    }
}
