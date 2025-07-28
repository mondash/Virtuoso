using UnityEngine;

namespace FooPlugin42.BuglePitch;

internal readonly struct BuglePitchFrame(BugleSFX bugle)
{
    public readonly int Valves = TrumpetValves.GetOffset();
    public readonly float Harmonic = BuglePitchInput.GetHarmonic(bugle);
    public readonly float Bend = BuglePitchInput.GetBend(bugle);
    public float Semitone => Valves + Harmonic + Bend;
    public float Pitch => BuglePitchMath.ConvertToPitch(Semitone);
    public bool Approximately(BuglePitchFrame other, float bendThreshold = 0.05f) =>
        Valves == other.Valves
        && Mathf.Approximately(Harmonic, other.Harmonic)
        && Mathf.Abs(Bend - other.Bend) < bendThreshold;
}
