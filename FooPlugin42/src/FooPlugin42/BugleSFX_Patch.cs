using System.Reflection;
using FooPlugin42.BuglePitch;
using HarmonyLib;

namespace FooPlugin42;

[HarmonyPatch(typeof(BugleSFX))]
internal static class BugleSFX_Patch
{
    [HarmonyPatch(nameof(BugleSFX.RPC_StartToot))]
    [HarmonyPostfix]
    private static void RPC_StartToot_Postfix(BugleSFX __instance, int clip)
    {
        BuglePitchStateManager.SetInitialHorizontal(__instance);

        var audioSource = __instance.buglePlayer;
        if (!audioSource) return;

        // Patch in custom sound
        audioSource.clip = BugleClip.Brass();
        // audioSource.pitch = new BuglePitchFrame(__instance).Pitch; // TODO Do I actually need this?
        audioSource.loop = true;
        audioSource.volume = 0f;
        audioSource.Play();

        // Mark sound as already playing so it's not overwritten in Update
        __instance
            .GetType()
            .GetField("t", BindingFlags.Instance | BindingFlags.NonPublic)
            ?.SetValue(__instance, true);
    }

    [HarmonyPatch(nameof(BugleSFX.RPC_EndToot))]
    [HarmonyPostfix]
    private static void RPC_EndToot_Postfix(BugleSFX __instance)
    {
        BuglePitchStateManager.Remove(__instance);
    }
}
