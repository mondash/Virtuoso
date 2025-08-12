using UnityEngine;
using Virtuoso.Config;

namespace Virtuoso.Input;

internal static class BuglePartial
{
    private const float SmoothStrength = 300f;
    private static float SmoothSpeed => SmoothStrength * BugleConfig.HarmonicSmoothMult.Value;
    public static float MaxAngle => BugleConfig.MaxPartialAngle.Value;
    private static bool Ideal => BugleConfig.UseIdealHarmonics.Value;
    private static readonly float[] RealisticHarmonics =
    [
        // 0.00f, // Fundamental // TODO Undo?
        12.00f, // +1 octave (total)
        19.02f, // ~7 semitones
        24.00f, // +2 octaves (total)
        27.86f, // ~4 semitones
        31.02f, // ~3 semitones
        33.69f, // ~3 semitones
        36.00f, // +3 octaves (total)
        // TODO Extend range?
    ];
    private static readonly float[] IdealHarmonics =
    [
        // 0f, // Fundamental // TODO Undo?
        12f, // +1 octave (total)
        19f, // +7 semitones
        24f, // +2 octaves (total)
        28f, // +4 semitones
        31f, // +3 semitones
        34f, // +3 semitones
        36f, // +3 octaves (total)
        // TODO Extend range?
    ];
    private static float? _smoothAngle;

    private static float[] Harmonics => Ideal ? IdealHarmonics : RealisticHarmonics;
    public static int Partials => Harmonics.Length;

    private static float CurrentAngle =>
        Character.localCharacter ? Character.localCharacter.data.lookValues.y : 0f;

    public static float Semitones()
    {
        var angle = _smoothAngle ?? CurrentAngle;
        var normalized = Mathf.InverseLerp(-MaxAngle, MaxAngle, angle);
        var scaled = Mathf.FloorToInt(normalized * Partials);
        // TODO Do I need this clamp? InverseLerp should return [0,1] so should double check
        var index = Mathf.Clamp(scaled, 0, Partials - 1);
        return Harmonics[index];
    }

    public static void Reset() => _smoothAngle = null;

    public static void Smooth(float delta) =>
        _smoothAngle = _smoothAngle.HasValue
            ? Mathf.MoveTowards(_smoothAngle.Value, CurrentAngle, delta * SmoothSpeed)
            : CurrentAngle;
}
