using UnityEngine;

namespace FooPlugin42.BuglePitch;

internal readonly struct BuglePitchFrame(int valves, float harmonic, float bend)
{
    public readonly int Valves = valves;
    public readonly float Harmonic = harmonic;
    public readonly float Bend = bend;

    public BuglePitchFrame(BugleSFX bugle) :
        this(
            TrumpetValves.GetOffset(),
            BuglePitchInput.GetHarmonic(bugle),
            BuglePitchInput.GetBend(bugle)
        ) { }

    public float Semitone => Valves + Harmonic + Bend;
    public float Pitch =>  Mathf.Pow(2f, Semitone / 12f);
    public bool Approximately(BuglePitchFrame other, float bendThreshold = 0.05f)
        => Valves == other.Valves
        && Mathf.Approximately(Harmonic, other.Harmonic)
        && Mathf.Abs(Bend - other.Bend) < bendThreshold;

    public float Smooth(float initial, float delta, float speed = 64f) =>
        Mathf.Lerp(initial, Pitch, delta * speed);
}
