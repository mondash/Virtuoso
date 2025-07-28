using System.Collections.Generic;
using UnityEngine;

namespace FooPlugin42.BuglePitch;

internal struct BuglePitchState
{
    public float? InitialHorizontal;
    public float? PreviousVertical;
}

internal static class BuglePitchStateManager
{
    private const float SmoothStrength = 32f;
    private static readonly Dictionary<int, BuglePitchState> States = new();

    public static void Remove(BugleSFX bugle) =>
        States.Remove(bugle.photonView.ViewID);

    private static BuglePitchState MaybeGetState(int id) =>
        States.GetValueOrDefault(id);

    public static void SetInitialHorizontal(BugleSFX bugle)
    {
        var id = bugle.photonView.ViewID;
        var state = MaybeGetState(id);

        state.InitialHorizontal = ViewAngle.Horizontal();
        States[id] = state;
    }

    // TODO Examine usage of this...
    public static float GetSmoothedVerticalAngle(BugleSFX bugle)
    {
        var current = ViewAngle.Vertical();

        var id = bugle.photonView.ViewID;
        var state = MaybeGetState(id);

        var smoothed = state.PreviousVertical is { } previous
            ? Mathf.Lerp(previous, current, Time.deltaTime * SmoothStrength)
            : current;

        // TODO Perhaps this should be moved to it's own function call
        state.PreviousVertical = smoothed;
        States[id] = state;

        return smoothed;
    }

    public static float GetHorizontalDelta(BugleSFX bugle)
    {
        var id = bugle.photonView.ViewID;
        var state = MaybeGetState(id);
        return state.InitialHorizontal is { } initial
            ? Mathf.DeltaAngle(initial, ViewAngle.Horizontal())
            : 0f;
    }
}
