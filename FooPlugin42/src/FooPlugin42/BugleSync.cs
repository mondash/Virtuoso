using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using FooPlugin42.BuglePitch;
using UnityEngine;

namespace FooPlugin42;

internal struct BugleState
{
    public BugleSFX Bugle;
    public BuglePitchFrame LastFrame;
    public float? RemotePitch;
    public float SendTimer;
}

internal class BugleSync : MonoBehaviourPun
{
    public static BugleSync? Instance;
    private static readonly Dictionary<int, BugleState> States = new();
    private const float SendInterval = 0.15f;

    public static void AddInstance(BugleSFX bugle)
    {
        var id = bugle.photonView.ViewID;
        var lastFrame = new BuglePitchFrame(bugle);
        States[id] = new BugleState { Bugle = bugle, LastFrame = lastFrame };
    }

    public static void RemoveInstance(BugleSFX bugle) =>
        States.Remove(bugle.photonView.ViewID);

    public static float? GetRemotePitch(int id) =>
        States.GetValueOrDefault(id).RemotePitch;

    public static IEnumerator WaitForSceneLoadAndInit(Plugin plugin)
    {
        Plugin.Log.LogInfo("Waiting for scene to load...");
        while (!Character.localCharacter) yield return null;
        Plugin.Log.LogInfo("Scene loaded, initializing BugleSync");
        Instance = plugin.gameObject.AddComponent<BugleSync>();
        Plugin.Log.LogInfo("BugleSync initialized");
    }

    private void OnDestroy()
    {
        Instance = null;
        Plugin.Log.LogInfo("BugleSync destroyed");
    }

    private void UpdateBugleState(BugleState state)
    {
        var bugle = state.Bugle;
        if (!photonView.IsMine || !bugle || !bugle.buglePlayer || !bugle.hold) return;

        var frame = new BuglePitchFrame(bugle);

        var timeChanged = (state.SendTimer += Time.deltaTime) > SendInterval;
        var pitchChanged = !frame.Approximately(state.LastFrame);
        var shouldSync = timeChanged || pitchChanged;
        if (!shouldSync) return;

        var id = bugle.photonView.ViewID;
        photonView.RPC("RPC_BugleSync", RpcTarget.Others, id, frame.Pitch, state.SendTimer);

        state.LastFrame = frame;
        state.SendTimer = 0f;
    }

    private void Update()
    {
        Plugin.Log.LogInfo("BugleSync updating");
        foreach (var state in States.Values) UpdateBugleState(state);
    }

    [PunRPC]
    private void RPC_BugleSync(int id, float pitch, float delta)
    {
        var view = PhotonView.Find(id);
        if (!view || !view.TryGetComponent<BugleSFX>(out var bugle)) return;

        var state = States.GetValueOrDefault(id, new BugleState { Bugle = bugle });
        state.RemotePitch = pitch;
        state.SendTimer = delta;
        States[id] = state;
        Plugin.Log.LogInfo($"Synced bugle {id} to {pitch}"); // TODO Delete
    }
}
