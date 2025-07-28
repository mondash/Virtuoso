using UnityEngine;

namespace FooPlugin42.BuglePitch;

public static class BuglePitchInput
{
    public const float MaxVerticalAngle = 90f;
    private const float MaxBendAngle = 90f;
    private const float MaxBendSemitones = 2f;
    private static readonly float[] Harmonics =
    [
        // 0.00f, // Fundamental // TODO Undo?
        12.00f, // +1 octave
        19.02f, // ~7 semitones
        24.00f, // +2 octaves (total)
        27.86f, // ~4 semitones
        31.02f, // ~3 semitones
        33.69f, // ~3 semitones
        36.00f, // +3 octaves (total)
    ];

    public static int HarmonicsCount => Harmonics.Length;

    public static float GetHarmonic(BugleSFX instance)
    {
        var smooth = BuglePitchStateManager.GetSmoothedVerticalAngle(instance);
        var normalized = Mathf.InverseLerp(-MaxVerticalAngle, MaxVerticalAngle, smooth);
        var partial =  Mathf.FloorToInt(normalized * HarmonicsCount);
        var index = Mathf.Clamp(partial, 0, HarmonicsCount - 1);
        return Harmonics[index];
    }

    public static float GetBend(BugleSFX instance)
    {
        var delta = BuglePitchStateManager.GetHorizontalDelta(instance);
        var normalized = Mathf.Clamp(delta / MaxBendAngle, -1f, 1f);
        return normalized * MaxBendSemitones;
    }
}
