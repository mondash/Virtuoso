using UnityEngine;

namespace FooPlugin42.Audio;

internal static class BugleClip
{
    private static AudioClip? _clip;

    // TODO Configurable fundamental frequency
    public static AudioClip Brass(float frequency = 58.27f /* Bb1 */)
    {
        if (_clip) return _clip;

        Plugin.Log.LogInfo("Generating bugle brass clip");

        const int cycles = 10;
        const int sampleRate = 44100;
        var duration = cycles / frequency;
        var sampleCount = (int)(sampleRate * duration);
        var samples = new float[sampleCount];

        for (var i = 0; i < sampleCount; i++)
        {
            var t = 2 * Mathf.PI * frequency * i / sampleRate;
            var buzz = (
                Mathf.Sin(t)
                + 0.70f * Mathf.Sin(2*t)
                + 0.50f * Mathf.Sin(3*t)
                + 0.40f * Mathf.Sin(4*t)
                + 0.30f * Mathf.Sin(5*t)
                + 0.20f * Mathf.Sin(6*t)
                + 0.15f * Mathf.Sin(7*t)
                + 0.10f * Mathf.Sin(8*t)
            ) * 0.2f;
            var breath = 0.02f * (Random.value - 0.5f);

            samples[i] = buzz + breath;
        }

        _clip = AudioClip.Create("BugleBrassClip", sampleCount, 1, sampleRate, false);
        _clip.SetData(samples, 0);
        return _clip;
    }
}
