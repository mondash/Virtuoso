using System.Collections.Generic;
using UnityEngine;

namespace FooPlugin42;

internal static class TrumpetValves
{
    private static readonly Dictionary<int, int> ValveOffsets = new()
    {
        [0b000] = 0,
        [0b010] = -1,
        [0b001] = -2,
        [0b011] = -3,
        [0b110] = -4,
        [0b101] = -5,
        [0b111] = -6,
        [0b100] = -3
    };

    // TODO Use proper input keymapping
    public static int GetOffset()
    {
        var valves = 0;
        if (Input.GetKey(KeyCode.Z)) valves |= 0b001;
        if (Input.GetKey(KeyCode.X)) valves |= 0b010;
        if (Input.GetKey(KeyCode.C)) valves |= 0b100;
        return ValveOffsets.GetValueOrDefault(valves, 0);
    }
}