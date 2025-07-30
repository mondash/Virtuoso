using System.Collections.Generic;
using FooPlugin42.BuglePitch;
using Photon.Pun;
using UnityEngine;

namespace FooPlugin42;

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

        var frame = new BuglePitchFrame(bugle);
        var withinSendInterval = state.SendTimer < SendInterval;
        var pitchUnchanged = frame.Approximately(state.LastFrame);

        if (withinSendInterval && pitchUnchanged) return;

        // Apply pitch to local bugle
        bugle.buglePlayer.pitch =  frame.Pitch;

        // TODO Glide here?
        // var current = bugle.buglePlayer.pitch;
        // bugle.buglePlayer.pitch = BuglePitchMath.Glide(current, frame.Pitch, Time.deltaTime);

        // Sync frame with other clients
        SyncFrame(viewID, frame, state.SendTimer);

        state.LastFrame = frame;
        state.SendTimer = 0f;
    }

    // TODO BuglePitchFrame serialization?
    private void SyncFrame(int viewID, BuglePitchFrame frame, float delta)
    {
        Plugin.Log.LogInfo($"Syncing frame with other clients");
        const string rpc = nameof(RPC_SyncFrame);
        photonView.RPC(rpc, RpcTarget.Others, viewID, frame.Valves, frame.Harmonic, frame.Bend, delta);
    }

    [PunRPC]
    private void RPC_SyncFrame(int viewID, int valves, float harmonic, float bend, float delta)
    {
        Plugin.Log.LogInfo($"Received frame update for view {viewID}");
        // TODO Delete this
        if (photonView.ViewID == viewID)
        {
            Plugin.Log.LogInfo("Caught own rpc");
            return;
        }

        var view = PhotonView.Find(viewID);
        if (!view || !view.TryGetComponent<BugleSFX>(out var bugle)) return;
        if (!bugle.buglePlayer || !bugle.hold) return;

        var frame = new BuglePitchFrame(valves, harmonic, bend);

        // Apply pitch received from remote
        bugle.buglePlayer.pitch = frame.Pitch;

        // TODO Glide here?
        // var current = bugle.buglePlayer.pitch;
        // bugle.buglePlayer.pitch = BuglePitchMath.Glide(current, frame.Pitch, delta);

        if (!States.TryGetValue(viewID, out var state))
            States[viewID] = state = new BugleSyncState();

        state.LastFrame = frame;
        state.SendTimer = delta;
    }
}
