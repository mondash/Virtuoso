using System.Reflection;
using HarmonyLib;
using Virtuoso.Behaviour;
using Virtuoso.Event;

namespace Virtuoso.Patching;

[HarmonyPatch(typeof(BugleSFX))]
internal static class BugleSFX_Patch
{
    // Mark as already tooting so audio isn't overwritten in Update
    private static void SetTooting(BugleSFX instance) =>
        instance
            .GetType()
            .GetField("t", BindingFlags.Instance | BindingFlags.NonPublic)
            ?.SetValue(instance, true);

    [HarmonyPatch(nameof(BugleSFX.RPC_StartToot))]
    [HarmonyPostfix]
    private static void RPC_StartToot_Postfix(BugleSFX __instance, int clip, float pitch)
    {
        SetTooting(__instance);
        BugleBehaviour.Connect(__instance);
        BugleHub.RaiseTootStarted(__instance);
    }

    [HarmonyPatch(nameof(BugleSFX.RPC_EndToot))]
    [HarmonyPostfix]
    private static void RPC_EndToot_Postfix(BugleSFX __instance)
    {
        BugleHub.RaiseTootStopped(__instance);
    }
}
