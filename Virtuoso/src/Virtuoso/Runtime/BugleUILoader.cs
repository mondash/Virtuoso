using System.Collections;
using UnityEngine;
using Virtuoso.Config;
using Virtuoso.UI;

namespace Virtuoso.Runtime;

internal class BugleUILoader : MonoBehaviour
{
    private void OnEnable() => StartCoroutine(LoadUI());

    private void OnDisable() => BugleUI.DestroyInstance();

    private bool ReadyOrDisabled() => Character.localCharacter || !isActiveAndEnabled;

    private IEnumerator LoadUI()
    {
        Plugin.Log.LogInfo("Bugle UI loader routine started");
        yield return new WaitUntil(ReadyOrDisabled);

        if (isActiveAndEnabled) BugleUI.Initialize(gameObject);
        Plugin.Log.LogInfo("Bugle UI loader routine finished");
    }
}
