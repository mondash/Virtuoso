using BepInEx.Configuration;

namespace Virtuoso.Config;

internal sealed class Setting<T>(ConfigEntry<T> entry)
{
    public ConfigEntry<T> Entry { get; } = entry;

    public static implicit operator T(Setting<T> setting)
        => setting.Entry.Value;
    public static implicit operator Setting<T>(ConfigEntry<T> configEntry)
        => new(configEntry);
}
