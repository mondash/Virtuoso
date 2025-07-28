using System.Reflection;
using FooPlugin42.BuglePitch;
using HarmonyLib;
using UnityEngine;

namespace FooPlugin42;

[HarmonyPatch(typeof(BugleSFX))]
internal static class BugleSFX_Patch
{

    [HarmonyPatch(nameof(BugleSFX.RPC_StartToot))]
    [HarmonyPrefix]
    private static void RPC_StartToot_Prefix(BugleSFX __instance)
    {
        BuglePitchStateManager.SetInitialHorizontal(__instance);
        BugleSync.AddInstance(__instance);
    }

    [HarmonyPatch(nameof(BugleSFX.RPC_StartToot))]
    [HarmonyPostfix]
    private static void RPC_StartToot_Postfix(BugleSFX __instance, int clip)
    {
        var audioSource = __instance.buglePlayer;
        if (!audioSource) return;

        // Patch in custom sound
        audioSource.clip = BugleClip.Brass();
        audioSource.pitch = new BuglePitchFrame(__instance).Pitch;
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
        BugleSync.RemoveInstance(__instance);
    }

    [HarmonyPatch(nameof(BugleSFX.Update))]
    [HarmonyPostfix]
    private static void Update_Postfix(BugleSFX __instance)
    {
        if (Time.timeScale == 0f) return; // TODO Is this even right?
        if (!__instance.buglePlayer || !__instance.hold) return;

        var id = __instance.photonView.ViewID;
        var current = __instance.buglePlayer.pitch;
        var target = __instance.photonView.IsMine
            ? new BuglePitchFrame(__instance).Pitch
            : BugleSync.GetRemotePitch(id);
        if (target == null) return;

        // TODO Should I use SendTime for gliding when updating remote?
        __instance.buglePlayer.pitch = BuglePitchMath.Glide(current, target.Value, Time.deltaTime);
    }
}
