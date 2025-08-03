using System.Collections;
using UnityEngine;
using Virtuoso.Config;
using Virtuoso.UI;

namespace Virtuoso.Runtime;

internal class BugleUILoader : MonoBehaviour
{
    private static float PollInterval => BugleConfig.UILoadInterval.Value;

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
