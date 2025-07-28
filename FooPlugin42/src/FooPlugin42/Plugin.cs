using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace FooPlugin42;

// TODO Handling multiple bugles?

[BepInAutoPlugin]
public partial class Plugin : BaseUnityPlugin
{
    internal static ManualLogSource Log { get; private set; } = null!;
    private Harmony? _harmony;

    private void Awake()
    {
        Log = Logger;
        Log.LogInfo("Plugin loaded!");
        (_harmony = new Harmony(Info.Metadata.GUID)).PatchAll();
        Log.LogInfo("Harmony patches applied.");
        StartCoroutine(BugleSync.WaitForSceneLoadAndInit(this));
        StartCoroutine(BugleUI.WaitForSceneLoadAndInit(this));
    }

    private void OnDestroy()
    {
        Log.LogInfo("Plugin unloading...");
        Destroy(BugleSync.Instance);
        Destroy(BugleUI.Instance);
        _harmony?.UnpatchSelf();
        Log.LogInfo("Plugin unloaded");
    }
}
