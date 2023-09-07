using Microsoft.Extensions.Configuration;

namespace Reformat.Framework.Core.Common.Extensions.Config;

public static class ConfigExtensions
{
    public static string GetValue(this IConfiguration cfg, string key, string defaultValue = null)
    {
        return cfg.GetSection(key).Value ?? defaultValue;
    }
}