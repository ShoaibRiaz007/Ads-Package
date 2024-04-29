#if Unity
using SH.Ads.Base;
using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;

namespace SH.Ads.Unity
{
    /// <summary>
    /// Manager for Admob
    /// </summary>
    internal class Manager : BaseManager, IUnityAdsInitializationListener
    {
        bool completed = false;
        public void OnInitializationComplete()
        {
            completed = true;
        }

        public void OnInitializationFailed(UnityAdsInitializationError error, string message)
        {
            completed = true;
            Debug.LogError(error + " \n " + message);
        }

        internal override IEnumerator Initialize(Base.Advertiser advertiser, bool isForChildren, string ageGroup)
        {
            yield return IsNetworkAvailable();
            if (Advertisement.isSupported)
            {
                Debug.Log(Application.platform + " supported by Advertisement");
            }
            
            Advertisement.Initialize(advertiser.ID, AdSettings.TestMode, this);

            while (!completed)
                yield return null;

            foreach (var t in advertiser.Ads)
                t.Intialize(advertiser.advertiser);
        }
    }
}
#endif