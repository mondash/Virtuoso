using BepInEx;
using BepInEx.Logging;
using FooPlugin42.Runtime;
using HarmonyLib;

namespace FooPlugin42;

// TODO Does this handle multiple local bugles?

[BepInAutoPlugin]
public partial class Plugin : BaseUnityPlugin
{
    internal static ManualLogSource Log { get; private set; } = null!;
    private Harmony? _harmony;

    private void Awake()
    {
        Log = Logger;
        Log.LogDebug("Plugin waking...");
        Log.LogDebug("Applying harmony patches...");
        (_harmony = new Harmony(Info.Metadata.GUID)).PatchAll();
        Log.LogDebug("Adding runtime components...");
        gameObject.AddComponent<BugleConnector>();
        gameObject.AddComponent<BugleUILoader>();
        Log.LogInfo("Plugin awake!");
    }

    private void OnDestroy()
    {
        Log.LogDebug("Plugin destroying...");
        Log.LogDebug("Removing harmony patches...");
        _harmony?.UnpatchSelf();
        Log.LogInfo("Plugin destroyed!");
    }
}
