#if Facebook
using AudienceNetwork;
using SH.Ads.Base;
using System.Collections;
using UnityEngine;

namespace SH.Ads.Facebook
{
    /// <summary>
    /// Manager for facebook
    /// </summary>
    internal class Manager : BaseManager
    {
        internal override IEnumerator Initialize(Base.Advertiser advertiser, bool isForChildren, string ageGroup)
        {
             yield return IsNetworkAvailable();


            AudienceNetwork.AdSettings.SetMixedAudience(isForChildren);
            // Add test device ID.
            if (AdSettings.TestMode)
            {
                if (!AdSettings.TestDevices.Contains(SystemInfo.deviceUniqueIdentifier.ToUpper().Trim()))
                    AdSettings.TestDevices.Add(SystemInfo.deviceUniqueIdentifier.ToUpper().Trim());
            }
            foreach (var id in AdSettings.TestDevices)
                AudienceNetwork.AdSettings.AddTestDevice(id);

            AudienceNetworkAds.Initialize();
            while(!AudienceNetworkAds.IsInitialized())
                yield return null;

            foreach (var t in advertiser.Ads)
                t.Intialize(advertiser.advertiser);
            yield return null;
        }
    }
}
#endif