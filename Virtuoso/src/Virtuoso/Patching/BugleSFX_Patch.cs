using HarmonyLib;
using Virtuoso.Behaviour;
using Virtuoso.Event;

namespace Virtuoso.Patching;

[HarmonyPatch(typeof(BugleSFX))]
internal static class BugleSFX_Patch
{
    [HarmonyPatch(nameof(BugleSFX.RPC_StartToot))]
    [HarmonyPostfix]
    private static void RPC_StartToot_Postfix(BugleSFX __instance, int clip, float pitch)
    {
        BugleBehaviour.Connect(__instance);
        BugleHub.RaiseTootStarted(__instance);
        __instance.t = true; // Prevents BugleSFX.Update from overwriting audio
    }

    [HarmonyPatch(nameof(BugleSFX.RPC_EndToot))]
    [HarmonyPostfix]
    private static void RPC_EndToot_Postfix(BugleSFX __instance)
    {
        BugleHub.RaiseTootStopped(__instance);
    }
}
