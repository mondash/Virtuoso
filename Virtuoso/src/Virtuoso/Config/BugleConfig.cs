using BepInEx.Configuration;
using UnityEngine;

namespace Virtuoso.Config;

internal static class BugleConfig
{
    public static ConfigEntry<float> SyncInterval = null!;

    public static ConfigEntry<float> UILoadInterval = null!;
    public static ConfigEntry<KeyCode> ToggleUIKey = null!;

    public static ConfigEntry<float> MaxBendAngle = null!;
    public static ConfigEntry<float> MaxBendSemitones = null!;

    public static ConfigEntry<bool> UseIdealHarmonics = null!;
    public static ConfigEntry<float> MaxPartialAngle = null!;

    public static ConfigEntry<bool> UseIdealValves = null!;
    public static ConfigEntry<KeyCode> Valve1Key = null!;
    public static ConfigEntry<KeyCode> Valve2Key = null!;
    public static ConfigEntry<KeyCode> Valve3Key = null!;

    public static void Bind(ConfigFile config)
    {
        SyncInterval = config.Bind(
            "Sync",
            "SyncInterval",
            0.1f,
            "Time (in seconds) between bugle sync frames"
        );

        UILoadInterval = config.Bind(
            "UI",
            "UILoadInterval",
            1.0f,
            "Interval (in seconds) between bugle UI load attempts"
        );
        ToggleUIKey = config.Bind(
            "UI",
            "ToggleUIKey",
            KeyCode.V,
            "Keyboard shortcut to toggle the bugle UI visibility"
        );

        MaxBendAngle = config.Bind(
            "Bend",
            "MaxBendAngle",
            80f,
            "Maximum left/right view angle (in degrees) allowed to bend pitch"
        );
        MaxBendSemitones = config.Bind(
            "Bend",
            "MaxBendSemitones",
            2f,
            "Maximum pitch bending amount in semitones"
        );

        UseIdealHarmonics = config.Bind(
            "Harmonics",
            "UseIdealHarmonics",
            true,
            "If true, uses mathematically ideal harmonics"
        );
        MaxPartialAngle = config.Bind(
            "Harmonics",
            "MaxPartialAngle",
            80f,
            "Vertical angle span (in degrees) allocated to partial selection"
        );

        UseIdealValves = config.Bind(
            "Valves",
            "UseIdealValves",
            true,
            "If true, uses mathematically ideal tuning offsets for valve combinations"
        );
        Valve1Key = config.Bind(
            "Valves",
            "Valve1Key",
            KeyCode.Z,
            "Keyboard key for Valve 1"
        );
        Valve2Key = config.Bind(
            "Valves",
            "Valve2Key",
            KeyCode.X,
            "Keyboard key for Valve 2"
        );
        Valve3Key = config.Bind(
            "Valves",
            "Valve3Key",
            KeyCode.C,
            "Keyboard key for Valve 3"
        );
    }
}
