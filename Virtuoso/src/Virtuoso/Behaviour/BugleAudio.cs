using System.Collections;
using UnityEngine;
using Virtuoso.Audio;
using Virtuoso.Config;
using Virtuoso.Data;

namespace Virtuoso.Behaviour;

[DefaultExecutionOrder(2)]
internal class BugleAudio : BugleBehaviour
{
    private const float VolumeEpsilon = 0.001f; // TODO Necessary?
    private const float SmoothStrength = 100f;
    private static float SmoothSpeed => SmoothStrength * BugleConfig.PitchSmoothMult.Value;

    private bool _newToot;
    private BuglePitchFrame _frame;
    private Coroutine? _stopRoutine;

    private AudioSource Source => Sfx.buglePlayer;

    protected override void OnTootStarted()
    {
        Source.clip = BugleClip.Pinched();
        Source.loop = true;
        _newToot = true;
    }

    private bool SourceActive() => isActiveAndEnabled && Source is { isPlaying: true, volume: > VolumeEpsilon };
    private IEnumerator StopRoutine()
    {
        yield return new WaitWhile(SourceActive);
        if (Source && Source.isPlaying) Source.Stop();
        _stopRoutine = null;
    }

    protected override void OnTootStopped() =>
        _stopRoutine ??= StartCoroutine(StopRoutine());

    protected override void OnFrame(BuglePitchFrame frame) => _frame = frame;

    private bool Pause()
    {
        if (Time.timeScale != 0f) return false;
        if (Source.isPlaying) Source.Stop();
        return true;
    }

    protected override bool ShouldUpdate => base.ShouldUpdate || Source.isPlaying;
    protected override void OnUpdate()
    {
        if (Pause()) return;

        // TODO Dynamic clip assignment
        Source.pitch = _newToot
            ? _frame.Pitch : _frame.Smooth(Source.pitch, Time.deltaTime, SmoothSpeed);
        if (!Source.isPlaying) Source.Play();
        _newToot = false;
    }
}
