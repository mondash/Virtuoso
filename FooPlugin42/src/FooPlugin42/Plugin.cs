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
            var bugles = FindObjectsByType<BugleSFX>(FindObjectsSortMode.None);
            foreach (var bugle in bugles) BugleSync.EnsureSync(bugle);
            yield return new WaitForSeconds(BugleConnectPollInterval);
        }
        Log.LogInfo("Bugle sync routine finished");
    }

    private static IEnumerator BugleUIRoutine()
    {
        Log.LogInfo("Bugle UI routine starting...");
        while (!_instance || !Character.localCharacter) yield return null;
        BugleUI.Initialize(_instance.gameObject);
        Log.LogInfo("Bugle UI routine finished");
    }

    private void OnDestroy()
    {
        Log.LogInfo("Plugin destroying...");
        _instance = null;
        Log.LogInfo("Stopping coroutines...");
        if (_bugleUIRoutine != null) StopCoroutine(_bugleUIRoutine);
        if (_bugleSyncRoutine != null) StopCoroutine(_bugleSyncRoutine);
        Destroy(BugleUI.Instance); // TODO Do I need to call this directly?
        Log.LogInfo("Removing harmony patches...");
        _harmony?.UnpatchSelf();
        Log.LogInfo("Plugin destroyed!");
    }
}
