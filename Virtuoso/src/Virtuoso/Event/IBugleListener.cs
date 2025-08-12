using Virtuoso.Data;

namespace Virtuoso.Event;

internal interface IBugleListener
{
    void HandleTootStarted();
    void HandleTootStopped();
    void HandleFrame(BuglePitchFrame frame);
}
