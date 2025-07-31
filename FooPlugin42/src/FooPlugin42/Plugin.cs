using System.Collections;
using BepInEx;
using BepInEx.Logging;
using FooPlugin42.Runtime;
using HarmonyLib;

namespace FooPlugin42;

// TODO Does this handle multiple bugles?

[BepInAutoPlugin]
public partial class Plugin : BaseUnityPlugin
{
    internal static ManualLogSource Log { get; private set; } = null!;
    private Harmony? _harmony;

    private void Awake()
    {
        Log = Logger;
        Log.LogInfo("Plugin waking...");
        Log.LogInfo("Applying harmony patches...");
        (_harmony = new Harmony(Info.Metadata.GUID)).PatchAll();
        Log.LogInfo("Adding runtime components...");
        gameObject.AddComponent<BugleConnector>();
        gameObject.AddComponent<BugleUILoader>();
        Log.LogInfo("Plugin awake!");
    }

    private void OnDestroy()
    {
        Log.LogInfo("Plugin destroying...");
        Log.LogInfo("Removing harmony patches...");
        _harmony?.UnpatchSelf();
        Log.LogInfo("Plugin destroyed!");
    }
}
