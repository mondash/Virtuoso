using UnityEngine;

namespace FooPlugin42.Input;

internal static class BuglePartial
{
    private const bool Ideal = false; // TODO Make configurable
    public const float MaxAngle = 90f;
    private const float SmoothStrength = 32f;
    // TODO Option for realistic vs quantized?
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
        if (!_smoothAngle.HasValue) return Harmonics[0];
        var normalized = Mathf.InverseLerp(-MaxAngle, MaxAngle, _smoothAngle.Value);
        var scaled = Mathf.FloorToInt(normalized * Partials);
        var index = Mathf.Clamp(scaled, 0, Partials - 1);
        return Harmonics[index];
    }

    public static void Reset() => _smoothAngle = null;

    public static void Smooth(float delta) =>
        _smoothAngle = _smoothAngle.HasValue
            ? Mathf.Lerp(_smoothAngle.Value, CurrentAngle, delta * SmoothStrength)
            : CurrentAngle;
}
