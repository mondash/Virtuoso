using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Virtuoso.Config;
using Virtuoso.Runtime;

namespace Virtuoso;

// TODO Should I try/catch anything anywhere?

[BepInAutoPlugin]
public partial class Plugin : BaseUnityPlugin
{
    internal static ManualLogSource Log { get; private set; } = null!;
    private Harmony? _harmony;

    private void Awake()
    {
        Log = Logger;
        Log.LogInfo("Plugin waking...");
        Log.LogDebug("Loading configuration...");
        BugleConfig.Bind(Config);
        Log.LogDebug("Applying harmony patches...");
        (_harmony = new Harmony(Info.Metadata.GUID)).PatchAll();
        Log.LogDebug("Adding runtime components...");
        gameObject.AddComponent<BugleConnector>();
        gameObject.AddComponent<BugleUILoader>();
        Log.LogInfo("Plugin awake!");
    }

    private void OnDestroy()
    {
        Log.LogInfo("Plugin destroying...");
        Log.LogDebug("Removing harmony patches...");
        _harmony?.UnpatchSelf();
        Log.LogInfo("Plugin destroyed!");
    }
}
