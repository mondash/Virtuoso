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
    private BuglePitchFrame _local, _sync;

    protected override bool ShouldHandleFrame => base.ShouldHandleFrame && IsMine;
    protected override void OnFrame(BuglePitchFrame frame) => _local = frame;

    private bool Throttle => _since < SyncInterval && _local.Approximately(_sync);
    protected override bool ShouldUpdate => base.ShouldUpdate && IsMine;
    protected override void OnUpdate()
    {
        _since += Time.deltaTime;
        if (Throttle) return;
        SyncBuglePitchFrame(_local);
    }

    private void SyncBuglePitchFrame(BuglePitchFrame frame)
    {
        Sfx.photonView.RPC(nameof(RPC_SyncBuglePitchFrame), RpcTarget.Others, frame.Data);
        Plugin.Log.LogDebug($"Sent frame sync from view {ViewID}");
        _sync = frame;
        _since = 0f;
    }

    [PunRPC]
    private void RPC_SyncBuglePitchFrame(object[] data, PhotonMessageInfo info)
    {
        Plugin.Log.LogDebug($"Received frame sync from actor {info.Sender.ActorNumber}");
        Hub.RaiseFrame(new BuglePitchFrame(data));
    }
}
