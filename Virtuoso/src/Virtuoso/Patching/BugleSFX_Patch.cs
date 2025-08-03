using System.Reflection;
using HarmonyLib;
using Virtuoso.Audio;
using Virtuoso.Input;

namespace Virtuoso.Patching;

[HarmonyPatch(typeof(BugleSFX))]
internal static class BugleSFX_Patch
{
    [HarmonyPatch(nameof(BugleSFX.RPC_StartToot))]
    [HarmonyPostfix]
    private static void RPC_StartToot_Postfix(BugleSFX __instance, int clip)
    {
        BugleBend.SetInitialAngle();

        var audioSource = __instance.buglePlayer;
        if (!audioSource) return;

        // Patch in custom sound
        audioSource.clip = BugleClip.Pinched();
        // audioSource.pitch = new BuglePitchFrame(__instance).Pitch; // TODO Do I actually need this?
        audioSource.loop = true; // TODO Synthesis without looping, involves updated sync behaviour
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
        if (!__instance.photonView.IsMine) return; // TODO Paranoid?
        BugleBend.Reset();
        BuglePartial.Reset();
    }
}
