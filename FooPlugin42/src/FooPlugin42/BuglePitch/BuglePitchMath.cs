using UnityEngine;

namespace FooPlugin42.BuglePitch;

public static class BuglePitchMath
{
    public static float Glide(float current, float target, float delta, float speed = 64f) =>
        Mathf.Lerp(current, target, delta * speed);

    public static float ConvertToPitch(float semitone) => Mathf.Pow(2f, semitone / 12f);

    public static float ConvertToSemitone(float pitch) => 12f * Mathf.Log(pitch, 2f);
}
