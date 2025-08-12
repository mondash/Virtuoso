using UnityEngine;

namespace Virtuoso.Behaviour;

[DefaultExecutionOrder(4)]
internal class BugleDrop : BugleBehaviour
{
    protected override void OnEnable()
    {
        Sfx.item.OnStateChange += DisconnectOnDrop;
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        Sfx.item.OnStateChange -= DisconnectOnDrop;
        base.OnDisable();
    }

    private void DisconnectOnDrop(ItemState state)
    {
        if (state != ItemState.Held) Disconnect(Sfx);
    }
}
