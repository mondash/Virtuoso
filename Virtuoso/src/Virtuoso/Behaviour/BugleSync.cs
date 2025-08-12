using Photon.Pun;
using UnityEngine;
using Virtuoso.Config;
using Virtuoso.Data;

namespace Virtuoso.Behaviour;

[DefaultExecutionOrder(3)]
internal class BugleSync : BugleBehaviour
{
    private static float SyncInterval => BugleConfig.SyncInterval.Value;

    private float _since;
    private BuglePitchFrame _lastLocal, _lastSent;

    protected override bool ShouldHandleFrame => base.ShouldHandleFrame && IsMine;
    protected override void OnFrame(BuglePitchFrame frame) => _lastLocal = frame;

    protected override bool ShouldUpdate => base.ShouldUpdate && IsMine;
    protected override void OnUpdate()
    {
        _since += Time.deltaTime;
        var throttle = _since < SyncInterval && _lastSent.Approximately(_lastLocal);
        if (throttle) return;
        SyncBuglePitchFrame(_lastLocal);
    }

    private void SyncBuglePitchFrame(BuglePitchFrame frame)
    {
        Sfx.photonView.RPC(nameof(RPC_SyncBuglePitchFrame), RpcTarget.Others, frame.Data);
        Plugin.Log.LogDebug($"Sent frame sync from view {ViewID}");
        _lastSent = frame;
        _since = 0f;
    }

    [PunRPC]
    private void RPC_SyncBuglePitchFrame(object[] data, PhotonMessageInfo info)
    {
        Plugin.Log.LogDebug($"Received frame sync from actor {info.Sender.ActorNumber}");
        Hub.RaiseFrame(new BuglePitchFrame(data));
    }
}
