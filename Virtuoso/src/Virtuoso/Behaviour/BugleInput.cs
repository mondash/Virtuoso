using UnityEngine;
using Virtuoso.Data;
using Virtuoso.Input;

namespace Virtuoso.Behaviour;

[DefaultExecutionOrder(1)]
internal class BugleInput : BugleBehaviour
{
    protected override bool ShouldHandleTootStarted => base.ShouldHandleTootStarted && IsMine;
    protected override void OnTootStarted()
    {
        BugleBend.SetInitialAngle();
        Hub.RaiseFrame(new BuglePitchFrame());
    }

    protected override bool ShouldHandleTootStopped => base.ShouldHandleTootStopped && IsMine;
    protected override void OnTootStopped()
    {
        BugleBend.Reset();
        BuglePartial.Reset();
    }

    protected override bool ShouldUpdate => base.ShouldUpdate && IsMine;
    protected override void OnUpdate()
    {
        BuglePartial.Smooth(Time.deltaTime);
        Hub.RaiseFrame(new BuglePitchFrame());
    }
}
