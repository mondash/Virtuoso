using UnityEngine;

namespace FooPlugin42;

public class NoteName
{
    private static readonly string[] NoteNames =
    [
        "C", "C#", "D", "D#", "E", "F",
        "F#", "G", "G#", "A", "A#", "B"
    ];

    public static string FromPitch(float pitch) => FromSemitone(BuglePitch.BuglePitchMath.ConvertToSemitone(pitch));

    // TODO Verify this
    public static string FromSemitone(float semitone)
    {
        var midi = Mathf.RoundToInt(semitone) + 69;

        // Clamp to MIDI note range
        midi = Mathf.Clamp(midi, 0, 127);

        // MIDI 60 = C4, so MIDI 0 = C-1
        var octave = midi / 12 - 1;
        var noteIndex = midi % 12;
        return $"{NoteNames[noteIndex]}{octave}";
    }
}
