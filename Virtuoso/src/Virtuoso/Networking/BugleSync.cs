using System.Linq;
using Photon.Pun;
using UnityEngine;
using Virtuoso.Config;
using Virtuoso.Data;

// TODO Might update this namespace
// Not *exactly* just networking
namespace Virtuoso.Networking;

internal class BugleSync: MonoBehaviourPun
{
    private float _timeSinceSync;
    private BuglePitchFrame _frame;

    private static float SyncInterval => BugleConfig.SyncInterval.Value;

    private void OnEnable() => Plugin.Log.LogInfo($"Enabled bugle sync for view {photonView.ViewID}");

    private void OnDisable() => Plugin.Log.LogInfo($"Disabled bugle sync for view {photonView.ViewID}");

    public static BugleSync Connect(BugleSFX bugle) =>
        bugle.TryGetComponent<BugleSync>(out var sync)
            ? sync
            : bugle.gameObject.AddComponent<BugleSync>();

    public static void DisconnectAll() =>
        FindObjectsByType<BugleSync>(FindObjectsSortMode.None)
            .ToList()
            .ForEach(Destroy);

    private void Update()
    {
        if (Time.timeScale == 0f) return; // TODO Should I include this here (or anywhere)?

        _timeSinceSync += Time.deltaTime;

        if (!TryGetComponent<BugleSFX>(out var bugle)) return;
        if (!bugle.hold || !bugle.buglePlayer) return;

        // TODO Move this into BugleSFX Update Postfix?
        bugle.buglePlayer.pitch = _frame.Smooth(bugle.buglePlayer.pitch, _timeSinceSync);

        if (!photonView.IsMine) return;

        var frame = new BuglePitchFrame();
        var withinSyncInterval = _timeSinceSync < SyncInterval;
        var pitchUnchanged = frame.Approximately(_frame);

        _frame = frame;

        if (withinSyncInterval && pitchUnchanged) return;

        var viewID = photonView.ViewID;
        photonView.RPC(nameof(RPC_SyncBuglePitchFrame), RpcTarget.Others, viewID, frame.Data);
        Plugin.Log.LogDebug($"Sent frame sync from view {viewID}");

        _timeSinceSync = 0f;
    }

    [PunRPC]
    private void RPC_SyncBuglePitchFrame(int viewID, object[] data)
    {
        Plugin.Log.LogDebug($"Received frame sync from view {viewID}");
        var view = PhotonView.Find(viewID);
        if (!view || view.IsMine) return;

        Plugin.Log.LogDebug($"Applying frame sync from view {viewID}");
        _frame = new BuglePitchFrame(data);
        _timeSinceSync = 0f;
    }
}
