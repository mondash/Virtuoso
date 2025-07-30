using System.Collections.Generic;
using UnityEngine;

namespace FooPlugin42.Input;

internal static class ViewAngle
{
    private const float SmoothStrength = 32f;
    private static readonly Dictionary<int, float> InitialHorizontal = new();
    private static readonly Dictionary<int, float> SmoothedVertical = new();

    public static float Horizontal()
    {
        var c = Character.localCharacter;
        return c ? c.data.lookValues.x : 0;
    }

    public static float Vertical()
    {
        var c = Character.localCharacter;
        return c ? c.data.lookValues.y : 0;
    }

    public static void Remove(BugleSFX bugle)
    {
        var id = bugle.photonView.ViewID;
        InitialHorizontal.Remove(id);
        SmoothedVertical.Remove(id);
    }

    public static float GetInitialHorizontal(BugleSFX bugle) =>
        InitialHorizontal[bugle.photonView.ViewID];

    public static void SetInitialHorizontal(BugleSFX bugle) =>
        InitialHorizontal[bugle.photonView.ViewID] = Horizontal();

    public static float GetHorizontalDelta(BugleSFX bugle) =>
        Mathf.DeltaAngle(GetInitialHorizontal(bugle), Horizontal());

    public static float GetSmoothVertical(BugleSFX bugle) =>
        SmoothedVertical[bugle.photonView.ViewID];

    public static void SetSmoothVertical(BugleSFX bugle, float delta) =>
        SmoothedVertical[bugle.photonView.ViewID] =
            Mathf.Lerp(GetSmoothVertical(bugle), Vertical(), delta * SmoothStrength);

    // TODO Would these be useful?
    // public static float Vertical(Component component) =>
    //     component ? component.transform.eulerAngles.y : 0;
    //
    // public static float Horizontal(Component component) =>
    //     component ? component.transform.eulerAngles.x : 0;
}
