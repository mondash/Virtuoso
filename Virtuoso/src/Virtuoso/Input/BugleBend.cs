using UnityEngine;
using Virtuoso.Config;

namespace Virtuoso.Input;

public static class BugleBend
{
    private static float MaxAngle => BugleConfig.MaxBendAngle.Value;
    private static float MaxSemitones => BugleConfig.MaxBendSemitones.Value;
    private static float? _initialAngle;

    private static float CurrentAngle =>
        // TODO Is this paranoid? Should I even bother checking?
        Character.localCharacter ? Character.localCharacter.data.lookValues.x : 0f;

    public static float Semitones()
    {
        if (!_initialAngle.HasValue) return 0f;
        var deltaAngle = Mathf.DeltaAngle(_initialAngle.Value, CurrentAngle);
        var delta = Mathf.Clamp(deltaAngle / MaxAngle, -1f, 1f);
        return delta * MaxSemitones;
    }

    public static void Reset() => _initialAngle = null;

    public static void SetInitialAngle() => _initialAngle = CurrentAngle;
}
