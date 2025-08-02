using System.Collections;
using FooPlugin42.Config;
using FooPlugin42.Networking;
using UnityEngine;

namespace FooPlugin42.Runtime;

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
