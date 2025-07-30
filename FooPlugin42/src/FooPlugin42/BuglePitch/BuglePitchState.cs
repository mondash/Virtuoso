using System.Collections.Generic;
using UnityEngine;

namespace FooPlugin42.BuglePitch;

// TODO Should this and does this work for non-player bugles?
// I don't think it's necessary for non-player bugles

// TODO Should this be a class?
internal struct BuglePitchState
{
    public float? InitialHorizontal;
    public float? PreviousVertical;
}

internal static class BuglePitchStateManager
{
    private const float SmoothStrength = 32f;
    private static readonly Dictionary<int, BuglePitchState> States = new();

    public static void Remove(BugleSFX bugle) => States.Remove(bugle.photonView.ViewID);

    public static void SetInitialHorizontal(BugleSFX bugle)
    {
        var id = bugle.photonView.ViewID;
        var state = States.GetValueOrDefault(id);

        state.InitialHorizontal = ViewAngle.Horizontal();
        States[id] = state;
    }

    // TODO Examine usage of this...
    public static float GetSmoothedVerticalAngle(BugleSFX bugle)
    {
        var current = ViewAngle.Vertical();

        var id = bugle.photonView.ViewID;
        var state = States.GetValueOrDefault(id);

        var smoothed = state.PreviousVertical is { } previous
            ? Mathf.Lerp(previous, current, Time.deltaTime * SmoothStrength)
            : current;

        // TODO Perhaps this should be moved to it's own function call
        state.PreviousVertical = smoothed;
        States[id] = state;

        return smoothed;
    }

    // TODO Does not work if you spin too far
    public static float GetHorizontalDelta(BugleSFX bugle)
    {
        var id = bugle.photonView.ViewID;
        var state = States.GetValueOrDefault(id);
        return state.InitialHorizontal is { } initial
            ? Mathf.DeltaAngle(initial, ViewAngle.Horizontal())
            : 0f;
    }
}
