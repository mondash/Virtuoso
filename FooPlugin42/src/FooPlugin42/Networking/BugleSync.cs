using System.Collections.Generic;
using System.Linq;
using FooPlugin42.Config;
using FooPlugin42.Data;
using FooPlugin42.Input;
using Photon.Pun;
using UnityEngine;

// TODO Might update this namespace
// Not *exactly* just networking
namespace FooPlugin42.Networking;

internal class BugleSyncState
{
    public BuglePitchFrame Frame;
    public float SendTimer;
}

internal class BugleSync: MonoBehaviourPun
{
    private static readonly Dictionary<int, BugleSyncState> States = new();
    private static float SendInterval => BugleConfig.SyncInterval.Value;

    private static void ConnectBugle(BugleSFX bugle)
    {
        if (bugle.TryGetComponent<BugleSync>(out _)) return;
        bugle.gameObject.AddComponent<BugleSync>();
        var viewID = bugle.photonView.ViewID;
        Plugin.Log.LogInfo($"Connected bugle sync for view {viewID}");
    }

    private static void DisconnectBugle(BugleSFX bugle)
    {
        if (!bugle.TryGetComponent<BugleSync>(out var bugleSync)) return;
        Destroy(bugleSync);
        var viewID = bugle.photonView.ViewID;
        States.Remove(viewID);
        Plugin.Log.LogInfo($"Disconnected bugle sync for view {viewID}");
    }

    public static void Connect() =>
        FindObjectsByType<BugleSFX>(FindObjectsSortMode.None)
            .ToList().ForEach(ConnectBugle);

    public static void Disconnect() =>
        FindObjectsByType<BugleSFX>(FindObjectsSortMode.None)
            .ToList().ForEach(DisconnectBugle);

    private static BugleSyncState LoadState(int viewID) =>
        States.TryGetValue(viewID, out var state)
            ? state
            : States[viewID] = new BugleSyncState();

    private void Update()
    {
        if (Time.timeScale == 0f) return; // TODO Should I include this here (or anywhere)?

        var viewID = photonView.ViewID;
        var state = LoadState(viewID);
        state.SendTimer += Time.deltaTime;

        // Discard state of inactive bugle
        if (!TryGetComponent<BugleSFX>(out var bugle) || !bugle.hold || !bugle.buglePlayer)
        {
            States.Remove(viewID);
            return;
        }

        // TODO Move this into BugleSFX Update Postfix?
        // TODO Smoothing isn't working properly on remote update
        bugle.buglePlayer.pitch = state.Frame.Pitch;
        // bugle.buglePlayer.pitch = state.Frame.Smooth(bugle.buglePlayer.pitch, Time.deltaTime);

        if (!photonView.IsMine) return;

        BuglePartial.Smooth(Time.deltaTime);

        var frame = new BuglePitchFrame();
        var withinSendInterval = state.SendTimer < SendInterval;
        var pitchUnchanged = frame.Approximately(state.Frame);

        state.Frame = frame;

        if (withinSendInterval && pitchUnchanged) return;

        photonView.RPC(nameof(RPC_SyncBuglePitchFrame), RpcTarget.Others, viewID, frame.Data);
        Plugin.Log.LogDebug($"Sent frame sync from view {viewID}");

        state.SendTimer = 0f;
    }

    [PunRPC]
    private void RPC_SyncBuglePitchFrame(int viewID, object[] data)
    {
        Plugin.Log.LogDebug($"Received frame sync from view {viewID}");
        var view = PhotonView.Find(viewID);
        if (!view || view.IsMine) return;

        Plugin.Log.LogDebug($"Applying frame sync from view {viewID}");
        LoadState(viewID).Frame = new BuglePitchFrame(data);
    }
}
