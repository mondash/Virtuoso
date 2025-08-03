using System.Collections.Generic;
using FooPlugin42.Config;
using UnityEngine;

namespace FooPlugin42.Input;

internal static class TrumpetValves
{
    private static bool Ideal => BugleConfig.UseIdealValves.Value;
    private static KeyCode Valve1Key => BugleConfig.Valve1Key.Value;
    private static KeyCode Valve2Key => BugleConfig.Valve2Key.Value;
    private static KeyCode Valve3Key => BugleConfig.Valve3Key.Value;
    private static readonly Dictionary<int, float> RealisticOffsets = new()
    {
        [0b000] = -0.00f,
        [0b010] = -1.00f,
        [0b001] = -2.00f,
        [0b100] = -2.95f,
        [0b011] = -3.00f,
        [0b110] = -4.00f,
        [0b101] = -5.35f,
        [0b111] = -6.75f
    };
    private static readonly Dictionary<int, float> IdealOffsets = new()
    {
        [0b000] = -0f,
        [0b010] = -1f,
        [0b001] = -2f,
        [0b100] = -3f,
        [0b011] = -3f,
        [0b110] = -4f,
        [0b101] = -5f,
        [0b111] = -6f
    };

    private static Dictionary<int, float> Offsets => Ideal ? IdealOffsets : RealisticOffsets;

    public static float Semitones()
    {
        var valves = 0;
        if (UnityEngine.Input.GetKey(Valve1Key)) valves |= 0b001;
        if (UnityEngine.Input.GetKey(Valve2Key)) valves |= 0b010;
        if (UnityEngine.Input.GetKey(Valve3Key)) valves |= 0b100;
        return Offsets.GetValueOrDefault(valves, 0);
    }
}
