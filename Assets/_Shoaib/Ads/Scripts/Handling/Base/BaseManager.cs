namespace SH.Ads.Base
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.Networking;

    /// <summary>
    /// Base class of every Manager suported ad advertiser
    /// </summary>
    internal abstract class BaseManager
    {
        internal abstract IEnumerator Initialize(Advertiser advertiser, bool isForChildren, string ageGroup);
        static bool IsConnected = false;
        public static IEnumerator IsNetworkAvailable()
        {
            WaitForSeconds wait = new WaitForSeconds(30);
            while (!IsConnected)
            {
                UnityWebRequest unityWebRequest = UnityWebRequest.Get("https://google.com");
                yield return unityWebRequest.SendWebRequest();
                if (unityWebRequest.result == UnityWebRequest.Result.Success)
                {
                    IsConnected=true;
                    Debug.Log("Ad Log : Is Connected To Internet");
                }
                else
                    yield return wait;
            }
        }
    }
}