using UnityEngine;

namespace Virtuoso.Audio;

internal static class BugleClip
{
    // TODO Configurable fundamental frequency
    private const float Frequency = 58.27f; // Bb1
    private static AudioClip? _clip;

    public static AudioClip Brass()
    {
        if (_clip) return _clip;

        Plugin.Log.LogInfo("Generating bugle brass clip");

        const int cycles = 10;
        const int sampleRate = 44100;
        const float duration = cycles / Frequency;
        const int sampleCount = (int)(sampleRate * duration);
        var samples = new float[sampleCount];

        for (var i = 0; i < sampleCount; i++)
        {
            var t = 2 * Mathf.PI * Frequency * i / sampleRate;
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

    public static AudioClip Brassier()
    {
        if (_clip) return _clip;

        Plugin.Log.LogInfo("Generating brassier clip");

        const int cycles = 10;
        const int sampleRate = 44100;
        const float duration = cycles / Frequency;
        const int sampleCount = (int)(sampleRate * duration);
        var samples = new float[sampleCount];

        for (var i = 0; i < sampleCount; i++)
        {
            var t = 2 * Mathf.PI * Frequency * i / sampleRate;

            // Carefully tuned harmonic stack (trumpet-style rolloff)
            var buzz =
                1.00f * Mathf.Sin(t) +
                0.80f * Mathf.Sin(2 * t) +
                1.20f * Mathf.Sin(3 * t) +  // bump
                0.60f * Mathf.Sin(4 * t) +
                0.40f * Mathf.Sin(5 * t) +
                0.80f * Mathf.Sin(6 * t) +  // bump
                0.20f * Mathf.Sin(7 * t) +
                0.10f * Mathf.Sin(8 * t);

            buzz *= 0.15f; // master gain

            // Gentle breath texture
            var breath = 0.01f * (Random.value - 0.5f);

            samples[i] = buzz + breath;
        }

        _clip = AudioClip.Create("BugleBrassierClip", sampleCount, 1, sampleRate, false);
        _clip.SetData(samples, 0);
        return _clip;
    }

    public static AudioClip Sawtooth()
    {
        if (_clip) return _clip;

        Plugin.Log.LogInfo("Generating sawtooth clip");

        const int cycles = 10;
        const int sampleRate = 44100;
        const float duration = cycles / Frequency;
        const int sampleCount = (int)(sampleRate * duration);
        var samples = new float[sampleCount];

        const int harmonics = 20;
        const float shimmerDetune = 1.002f;

        for (var i = 0; i < sampleCount; i++)
        {
            var t = 2 * Mathf.PI * Frequency * i / sampleRate;
            var shimmerT = 2 * Mathf.PI * Frequency * shimmerDetune * i / sampleRate;

            var value = 0f;

            for (var n = 1; n <= harmonics; n++)
            {
                // Base amplitude taper
                var weight = 1f / n * Mathf.Exp(-0.05f * n);

                // Add formant boost around 5th–7th harmonic
                if (n is >= 5 and <= 7) weight *= 1.5f;

                // Subtle harmonic wobble across cycle (not time)
                var cyclePhase = (float)i / sampleCount;
                var mod = 1f + 0.05f * Mathf.Sin(2 * Mathf.PI * n * cyclePhase);

                value += weight * mod * (
                    Mathf.Sin(n * t) + 0.3f * Mathf.Sin(n * shimmerT) // shimmer layer
                );
            }

            value *= 0.2f;

            var breath = 0.01f * (Random.value - 0.5f);
            samples[i] = value + breath;
        }

        _clip = AudioClip.Create("BugleSawtoothClip", sampleCount, 1, sampleRate, false);
        _clip.SetData(samples, 0);
        return _clip;
    }

    public static AudioClip Pinched()
    {
        if (_clip) return _clip;

        Plugin.Log.LogInfo("Generating fixed pinched brass clip");

        const int cycles = 10;
        const int sampleRate = 44100;
        const int sampleCount = (int)(sampleRate * cycles / Frequency);
        var samples = new float[sampleCount];

        const int harmonics = 20;

        // Formant boost range (approx. 1200–2500 Hz)
        // At Bb1, this is roughly harmonics 5–7
        const int formantMin = 5;
        const int formantMax = 7;

        for (var i = 0; i < sampleCount; i++)
        {
            var t = i / (float)sampleRate;
            var basePhase = 2 * Mathf.PI * Frequency * t;

            var value = 0f;

            for (var n = 1; n <= harmonics; n++)
            {
                var harmonicPhase = basePhase * n;

                // Soft pinch shaping (nonlinear — richer than pure sine)
                var sine = Mathf.Sin(harmonicPhase);
                var shaped = Mathf.Sign(sine) * Mathf.Pow(Mathf.Abs(sine), 1.1f); // pinch amount

                // Harmonic weight
                var weight = 1f / n * Mathf.Exp(-0.045f * n);

                // Formant bump around harmonics 5–7
                if (n is >= formantMin and <= formantMax) weight *= 1.5f;

                value += weight * shaped;
            }

            // Mild saturation to simulate horn compression (adds buzz edge)
            value += 0.15f * Mathf.Pow(value, 3f);

            // Slight breath noise
            var breath = 0.005f * (Random.value - 0.5f);
            samples[i] = value * 0.3f + breath;
        }

        _clip = AudioClip.Create("BuglePinchedClip", sampleCount, 1, sampleRate, false);
        _clip.SetData(samples, 0);
        return _clip;
    }
}
