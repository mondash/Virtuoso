using FooPlugin42.Input;
using UnityEngine;

namespace FooPlugin42.Data;

internal readonly struct BuglePitchFrame(float valves, float partial, float bend)
{
    public readonly float Valves = valves;
    public readonly float Partial = partial;
    public readonly float Bend = bend;

    public BuglePitchFrame() :
        this(TrumpetValves.Semitones(), BuglePartial.Semitones(), BugleBend.Semitones()) { }

    public BuglePitchFrame(object[] data) :
        this((float)data[0], (float)data[1], (float)data[2]) { }

    public object[] Data => [Valves, Partial, Bend];

    public float Semitone => Valves + Partial + Bend;

    public float Pitch => Mathf.Pow(2f, Semitone / 12f);

    public bool Approximately(BuglePitchFrame other, float bendThreshold = 0.05f)
        => Mathf.Approximately(Valves, other.Valves)
        && Mathf.Approximately(Partial, other.Partial)
        && Mathf.Abs(Bend - other.Bend) < bendThreshold;

    public float Smooth(float initial, float delta, float speed = 64f) =>
        Mathf.Lerp(initial, Pitch, delta * speed);
}
