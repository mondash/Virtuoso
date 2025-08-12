using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Virtuoso.Data;

namespace Virtuoso.Event;

internal sealed class BugleHub
{
    private event Action? TootStarted;
    private event Action? TootStopped;
    private event Action<BuglePitchFrame>? Frame;
    private readonly HashSet<IBugleListener> _listeners = [];

    public Action Subscribe(IBugleListener listener)
    {
        if (!_listeners.Add(listener)) return () => { };
        TootStarted += listener.HandleTootStarted;
        TootStopped += listener.HandleTootStopped;
        Frame += listener.HandleFrame;
        return () => Unsubscribe(listener);
    }

    private void Unsubscribe(IBugleListener listener)
    {
        if (!_listeners.Remove(listener)) return;
        TootStarted -= listener.HandleTootStarted;
        TootStopped -= listener.HandleTootStopped;
        Frame -= listener.HandleFrame;
    }

    private void RaiseTootStarted() => TootStarted?.Invoke();
    private void RaiseTootStopped() => TootStopped?.Invoke();
    public void RaiseFrame(BuglePitchFrame frame) => Frame?.Invoke(frame);

    private static readonly ConditionalWeakTable<BugleSFX, BugleHub> Registry = new();
    public static BugleHub Get(BugleSFX sfx) => Registry.GetValue(sfx, _ => new BugleHub());

    public static void RaiseTootStarted(BugleSFX sfx) => Get(sfx).RaiseTootStarted();
    public static void RaiseTootStopped(BugleSFX sfx) => Get(sfx).RaiseTootStopped();
}
