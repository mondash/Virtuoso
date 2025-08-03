using System.Collections;
using UnityEngine;
using Virtuoso.Config;
using Virtuoso.Networking;

namespace Virtuoso.Runtime;

internal class BugleConnector : MonoBehaviour
{
    private static float PollInterval => BugleConfig.ConnectInterval.Value;

    private void OnEnable() => StartCoroutine(MaintainConnection());

    private void OnDisable() => BugleSync.Disconnect();

    private IEnumerator MaintainConnection()
    {
        Plugin.Log.LogInfo("Bugle connector routine started");
        while (isActiveAndEnabled)
        {
            BugleSync.Connect();
            yield return new WaitForSeconds(PollInterval);
        }
        Plugin.Log.LogInfo("Bugle connector routine finished");
    }
}
