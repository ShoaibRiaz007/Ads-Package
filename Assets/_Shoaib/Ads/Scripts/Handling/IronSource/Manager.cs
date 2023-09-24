#if IronSource
using SH.Ads.Base;
using System.Collections;
using UnityEngine;

using IronsourceAgent = IronSource;

namespace SH.Ads.IronSource
{
    /// <summary>
    /// Manager for Ironsource
    /// </summary>
    internal class Manager : BaseManager
    {
        internal override IEnumerator Initialize(Advertiser advertiser, bool isForChildren, string ageGroup)
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

            foreach (var t in advertiser.Ads)
                t.Intialize(advertiser.advertiser);
            yield return null;
        }
    }
}
#endif