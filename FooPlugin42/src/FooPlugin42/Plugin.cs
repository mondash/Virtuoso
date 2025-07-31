using System.Collections;
using BepInEx;
using BepInEx.Logging;
using FooPlugin42.Networking;
using FooPlugin42.UI;
using HarmonyLib;
using UnityEngine;

namespace FooPlugin42;

// TODO Does this handle multiple bugles?

[BepInAutoPlugin]
public partial class Plugin : BaseUnityPlugin
{
    internal static ManualLogSource Log { get; private set; } = null!;
    private const float BugleSyncRoutineInterval = 1f;
    private const float BugleUIRoutineInterval = 1f;
    private static Plugin? _instance; // TODO Would prefer to delete
    private Coroutine? _bugleSyncRoutine;
    private Coroutine? _bugleUIRoutine;
    private Harmony? _harmony;

    private void Awake()
    {
        Log = Logger;
        Log.LogInfo("Plugin waking...");
        Log.LogInfo("Applying harmony patches...");
        (_harmony = new Harmony(Info.Metadata.GUID)).PatchAll();
        _instance = this;
        Log.LogInfo("Starting coroutines...");
        _bugleSyncRoutine = StartCoroutine(BugleSyncRoutine());
        _bugleUIRoutine = StartCoroutine(BugleUIRoutine());
        Log.LogInfo("Plugin awake!");
    }

    private static IEnumerator BugleSyncRoutine()
    {
        Log.LogInfo("Bugle sync routine starting...");
        while (_instance)
        {
            BugleSync.Connect();
            yield return new WaitForSeconds(BugleSyncRoutineInterval);
        }
        Log.LogInfo("Bugle sync routine finished");
    }

    private static IEnumerator BugleUIRoutine()
    {
        Log.LogInfo("Bugle UI routine starting...");
        while (_instance && !Character.localCharacter)
            yield return new WaitForSeconds(BugleUIRoutineInterval);
        if (_instance) BugleUI.Initialize(_instance.gameObject);
        Log.LogInfo("Bugle UI routine finished");
    }

    private void OnDestroy()
    {
        Log.LogInfo("Plugin destroying...");
        _instance = null;
        Log.LogInfo("Stopping coroutines...");
        // TODO Does this prevent the coroutines from finishing smoothly?
        if (_bugleUIRoutine != null) StopCoroutine(_bugleUIRoutine);
        if (_bugleSyncRoutine != null) StopCoroutine(_bugleSyncRoutine);
        BugleUI.DestroyInstance();
        BugleSync.Disconnect();
        Log.LogInfo("Removing harmony patches...");
        _harmony?.UnpatchSelf();
        Log.LogInfo("Plugin destroyed!");
    }
}
