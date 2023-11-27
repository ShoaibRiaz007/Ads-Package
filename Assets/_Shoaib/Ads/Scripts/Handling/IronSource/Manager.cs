#if IronSource
using SH.Ads.Base;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using IronsourceAgent = IronSource;

namespace SH.Ads.IronSource
{
    /// <summary>
    /// Manager for Ironsource
    /// </summary>
    internal class Manager : BaseManager
    {
        internal override IEnumerator Initialize(Base.Advertiser advertiser, bool isForChildren, string ageGroup)
        {
             yield return IsNetworkAvailable();

            // Add test device ID.
            if (AdSettings.TestMode)
            {
                if (!AdSettings.TestDevices.Contains(SystemInfo.deviceUniqueIdentifier.ToUpper().Trim()))
                    AdSettings.TestDevices.Add(SystemInfo.deviceUniqueIdentifier.ToUpper().Trim());
            }

            IronsourceAgent.Agent.setMetaData("is_child_directed",isForChildren? "true" : "false");
            IronsourceAgent.Agent.setMetaData("npa", AdSettings.GDPRConcent ? "0" : "1");
            IronsourceAgent.Agent.setMetaData("rdp", AdSettings.CCPAConsent ? "0" : "1");

            List<string> suportedAds = new List<string>();
            foreach (var t in advertiser.Ads)
            {
                switch (t.type) 
                {
                    case AdType.Banner:
                        suportedAds.Add(IronSourceAdUnits.BANNER);
                        break;
                    case AdType.BigBanner:
                        suportedAds.Add(IronSourceAdUnits.BANNER);
                        break;
                    case AdType.Rewarded:
                        suportedAds.Add(IronSourceAdUnits.REWARDED_VIDEO);
                        break;
                    case AdType.Interstial:
                        suportedAds.Add(IronSourceAdUnits.INTERSTITIAL);
                        break;
                }
            }
            bool initalized = false;
            IronSourceEvents.onSdkInitializationCompletedEvent += () => initalized = true;
            IronsourceAgent.Agent.init(advertiser.ID, suportedAds.ToArray());


            while (!initalized)
                yield return null;
           
            foreach (var t in advertiser.Ads)
                t.Intialize(advertiser.advertiser);
            yield return null;
        }
    }
}
#endif