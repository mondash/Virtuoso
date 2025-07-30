using System.Collections.Generic;
using FooPlugin42.Data;
using FooPlugin42.Input;
using Photon.Pun;
using UnityEngine;

// TODO Might update this namespace
// Not *exactly* just networking
namespace FooPlugin42.Networking;

internal class BugleSyncState
{
    public BuglePitchFrame LastFrame;
    public float SendTimer;
}

internal class BugleSync: MonoBehaviourPun
{
    private static readonly Dictionary<int, BugleSyncState> States = new();
    private const float SendInterval = 0.1f; // TODO Determine good default and maybe make configurable

    public static void EnsureSync(BugleSFX bugle)
    {
        var view = bugle.photonView;
        if (!view) return;
        // if (!view || !view.IsMine) return; // TODO All bugles should get a sync, right?
        if (bugle.TryGetComponent<BugleSync>(out _)) return;
        bugle.gameObject.AddComponent<BugleSync>();
        Plugin.Log.LogInfo($"Attached BugleSync to BugleSFX for view {view.ViewID}");
    }

    private void Update()
    {
        if (!photonView.IsMine) return;
        // if (Time.timeScale == 0f) return; // TODO Should I include this here?

        if (!TryGetComponent<BugleSFX>(out var bugle)) return;
        if (!bugle.hold || !bugle.buglePlayer) return;

        var viewID = bugle.photonView.ViewID;

        if (!States.TryGetValue(viewID, out var state))
            States[viewID] = state = new BugleSyncState();

        // TODO Should this happen before or after frame and does it matter?
        state.SendTimer += Time.deltaTime;

        // TODO Should probably move this to its own input behaviour
        BuglePartial.Smooth(Time.deltaTime);

        var frame = new BuglePitchFrame(bugle);

        // Apply pitch to local bugle
        bugle.buglePlayer.pitch = frame.Pitch;
        // TODO Smooth here?
        // var current = bugle.buglePlayer.pitch;
        // bugle.buglePlayer.pitch = frame.Smooth(current, Time.deltaTime);

        var withinSendInterval = state.SendTimer < SendInterval;
        var pitchUnchanged = frame.Approximately(state.LastFrame);
        if (withinSendInterval && pitchUnchanged) return;

        // Sync frame with other clients
        SyncFrame(viewID, frame, state.SendTimer);

        state.LastFrame = frame;
        state.SendTimer = 0f;
    }

    // TODO BuglePitchFrame serialization?
    private void SyncFrame(int viewID, BuglePitchFrame frame, float delta)
    {
        const string rpc = nameof(RPC_SyncFrame);
        photonView.RPC(rpc, RpcTarget.Others, viewID, frame.Valves, frame.Partial, frame.Bend, delta);
        Plugin.Log.LogDebug($"Sent frame sync from view {viewID}");
    }

    [PunRPC]
    private void RPC_SyncFrame(int viewID, float valves, float partial, float bend, float delta)
    {
        Plugin.Log.LogDebug($"Received frame sync from view {viewID}");
        var view = PhotonView.Find(viewID);
        if (!view || view.IsMine) return;
        if (!view.TryGetComponent<BugleSFX>(out var bugle)) return;
        if (!bugle.buglePlayer || !bugle.hold) return;

        Plugin.Log.LogDebug($"Applying frame sync from view {viewID}");
        var frame = new BuglePitchFrame(valves, partial, bend);

        // Apply pitch received from remote
        bugle.buglePlayer.pitch = frame.Pitch;
        // TODO Smooth here?
        // var current = bugle.buglePlayer.pitch;
        // bugle.buglePlayer.pitch = frame.Smooth(current, delta);

        if (!States.TryGetValue(viewID, out var state))
            States[viewID] = state = new BugleSyncState();

        state.LastFrame = frame;
        state.SendTimer = delta;
    }
}
