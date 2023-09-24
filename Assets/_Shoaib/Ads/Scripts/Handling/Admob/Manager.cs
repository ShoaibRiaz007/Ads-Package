#if Admob
using GoogleMobileAds.Api;
using SH.Ads.Base;
using System.Collections;
using UnityEngine;

namespace SH.Ads.Admob
{
    /// <summary>
    /// Manager for Admob
    /// </summary>
    internal class Manager : BaseManager
    {
        internal override IEnumerator Initialize(Advertiser advertiser, bool isForChildren, string ageGroup)
        {
            yield return IsNetworkAvailable();

            TagForChildDirectedTreatment tagFororChildren;
            TagForUnderAgeOfConsent tagForUnderAge;
            MaxAdContentRating contentRating;

            if (isForChildren)
            {
                tagFororChildren = TagForChildDirectedTreatment.True;
                tagForUnderAge = TagForUnderAgeOfConsent.True;
                contentRating = MaxAdContentRating.G;
            }
            else
            {
                tagFororChildren = TagForChildDirectedTreatment.Unspecified;
                tagForUnderAge = TagForUnderAgeOfConsent.Unspecified;
                contentRating = MaxAdContentRating.ToMaxAdContentRating(ageGroup);
            }


            var requestConfiguration = new RequestConfiguration();
            requestConfiguration.TagForChildDirectedTreatment = tagFororChildren;
            requestConfiguration.TagForUnderAgeOfConsent = tagForUnderAge;
            requestConfiguration.MaxAdContentRating = contentRating;
            // Add test device ID.
            if (AdSettings.TestMode)
            {
                if (!AdSettings.TestDevices.Contains(AdRequest.TestDeviceSimulator))
                    AdSettings.TestDevices.Add(AdRequest.TestDeviceSimulator);
                if (!AdSettings.TestDevices.Contains(SystemInfo.deviceUniqueIdentifier.ToUpper().Trim()))
                    AdSettings.TestDevices.Add(SystemInfo.deviceUniqueIdentifier.ToUpper().Trim());
            }
            requestConfiguration.TestDeviceIds = AdSettings.TestDevices;
            MobileAds.SetRequestConfiguration(requestConfiguration);
            MobileAds.SetiOSAppPauseOnBackground(true);
            bool completed = false;
            MobileAds.Initialize((status) =>
            {
                foreach (var map in status.getAdapterStatusMap())
                {
                    switch (map.Value.InitializationState)
                    {
                        case AdapterState.NotReady:
                            // The adapter initialization did not complete.
                            Debug.LogFormat("Adapter: {0} : {1} not ready.", map.Key, map.Value);
                            break;
                        case AdapterState.Ready:
                            // The adapter was successfully initialized.
                            Debug.LogFormat("Adapter: {0} : {1} ready.", map.Key, map.Value);
                            break;
                    }
                }
                completed=true;
                foreach (var tem in advertiser.Ads)
                    tem.Intialize(advertiser.advertiser);
            });

            while (!completed)
                yield return null;
        }
    }
}
#endif