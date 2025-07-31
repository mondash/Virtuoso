using System.Collections;
using FooPlugin42.UI;
using UnityEngine;

namespace FooPlugin42.Runtime;

internal class BugleUILoader : MonoBehaviour
{
    private const float PollInterval = 1f;

    private void OnEnable() => StartCoroutine(LoadUI());

    private void OnDisable() => BugleUI.DestroyInstance();

    private IEnumerator LoadUI()
    {
        Plugin.Log.LogInfo("Bugle UI loader routine started");
        while (isActiveAndEnabled && !Character.localCharacter)
            yield return new WaitForSeconds(PollInterval);

        if (isActiveAndEnabled) BugleUI.Initialize(gameObject);
        Plugin.Log.LogInfo("Bugle UI loader routine finished");
    }
}
