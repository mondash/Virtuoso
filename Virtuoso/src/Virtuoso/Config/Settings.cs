using BepInEx.Configuration;
using UnityEngine;

namespace Virtuoso.Config;

// TODO Smart use of modifier keys

internal static class Settings
{
    public static Setting<float> MaxBendAngle = null!;
    public static Setting<float> MaxBendSemitones = null!;

    public static Setting<bool> UseIdealHarmonics = null!;
    public static Setting<float> MaxPartialAngle = null!;

    public static Setting<float> HarmonicSmoothMult = null!;
    public static Setting<float> PitchSmoothMult = null!;

    public static Setting<float> SyncInterval = null!;

    public static Setting<KeyCode> ToggleUIKey = null!;

    public static Setting<bool> UseIdealValves = null!;
    public static Setting<KeyCode> Valve1Key = null!;
    public static Setting<KeyCode> Valve2Key = null!;
    public static Setting<KeyCode> Valve3Key = null!;

    public static void Bind(ConfigFile config)
    {
        MaxBendAngle = config.Bind(
            "Bend",
            "MaxBendAngle",
            80f,
            new ConfigDescription(
                "Maximum left/right view angle (in degrees) allowed to bend pitch",
                new AcceptableValueRange<float>(10f, 170f)
            )
        );
        MaxBendSemitones = config.Bind(
            "Bend",
            "MaxBendSemitones",
            2f,
            new ConfigDescription(
                "Maximum pitch bending amount in semitones",
                new AcceptableValueRange<float>(0f, 12f)
            )
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
            new ConfigDescription(
                "Vertical angle span (in degrees) allocated to partial selection",
                new AcceptableValueRange<float>(10f, 90f)
            )
        );

        HarmonicSmoothMult = config.Bind(
            "Smoothing",
            "HarmonicSmoothMult",
            1f,
            new ConfigDescription(
                "How quickly harmonic partials react, higher values reacting quicker",
                new AcceptableValueRange<float>(0.125f, 8f)
            )
        );
        PitchSmoothMult = config.Bind(
            "Smoothing",
            "PitchSmoothMult",
            1f,
            new ConfigDescription(
                "How quickly pitch changes are smoothed, higher values smoothing quicker",
                new AcceptableValueRange<float>(0.125f, 8f)
            )
        );

        SyncInterval = config.Bind(
            "Sync",
            "SyncInterval",
            0.1f,
            new ConfigDescription(
                "Time (in seconds) between bugle sync frames",
                new AcceptableValueRange<float>(0.01f, 1f)
            )
        );

        ToggleUIKey = config.Bind(
            "UI",
            "ToggleUIKey",
            KeyCode.U,
            "Keyboard shortcut to toggle the bugle UI visibility"
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
            KeyCode.C,
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
            KeyCode.Z,
            "Keyboard key for Valve 3"
        );
    }
}
