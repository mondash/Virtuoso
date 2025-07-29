using System.Collections;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace FooPlugin42;

// TODO Does this handle multiple bugles?

[BepInAutoPlugin]
public partial class Plugin : BaseUnityPlugin
{
    internal static ManualLogSource Log { get; private set; } = null!;
    private const float BugleConnectPollInterval = 1f;
    private static Plugin? _instance; // TODO Would prefer to delete
    private Coroutine? _bugleSyncRoutine;
    private Coroutine? _bugleUIInitRoutine;
    private Harmony? _harmony;

    private void Awake()
    {
        Log = Logger;
        Log.LogInfo("Plugin awake!");
        (_harmony = new Harmony(Info.Metadata.GUID)).PatchAll();
        Log.LogInfo("Harmony patches applied.");
        _instance = this;
        _bugleSyncRoutine = StartCoroutine(BugleSyncRoutine());
        _bugleUIInitRoutine = StartCoroutine(BugleUI.WaitForSceneLoadAndInit(this));
    }

    private static IEnumerator BugleSyncRoutine()
    {
        Log.LogInfo("Bugle sync routine starting...");
        while (_instance)
        {
            var bugles = FindObjectsByType<BugleSFX>(FindObjectsSortMode.None);
            foreach (var bugle in bugles) BugleSync.EnsureSync(bugle);
            yield return new WaitForSeconds(BugleConnectPollInterval);
        }
        Log.LogInfo("Bugle sync routine finished.");
    }

    private void OnDestroy()
    {
        Log.LogInfo("Plugin unloading...");
        _instance = null;
        Destroy(BugleUI.Instance);
        if (_bugleUIInitRoutine != null) StopCoroutine(_bugleUIInitRoutine);
        if (_bugleSyncRoutine != null) StopCoroutine(_bugleSyncRoutine);
        _harmony?.UnpatchSelf();
        Log.LogInfo("Plugin unloaded");
    }
}
