#if AdColony
using AdColony;
using SH.Ads.Base;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ColonyAd = AdColony.Ads;

namespace SH.Ads.AdColony
{
    class Manager : BaseManager
    {
        internal override IEnumerator Initialize(Base.Advertiser advertiser, bool isForChildren, string ageGroup)
        {
            yield return IsNetworkAvailable();

            //preparing AdColony SDK for initialization
            AppOptions appOptions = new AppOptions();
            appOptions.SetPrivacyFrameworkRequired(AppOptions.GDPR, AdSettings.GDPRConcent);
            appOptions.SetPrivacyConsentString(AppOptions.GDPR,AdSettings.GDPRConcent? "1" : "0");

            appOptions.SetPrivacyFrameworkRequired(AppOptions.CCPA, AdSettings.CCPAConsent);
            appOptions.SetPrivacyConsentString(AppOptions.CCPA,AdSettings.CCPAConsent? "1" : "0");
            
            appOptions.SetPrivacyFrameworkRequired(AppOptions.COPPA, AdSettings.IsForChildren);

            appOptions.TestModeEnabled = AdSettings.TestMode;
            // Add test device ID.
            if (AdSettings.TestMode)
            {
                if (!AdSettings.TestDevices.Contains(SystemInfo.deviceUniqueIdentifier.ToUpper().Trim()))
                    AdSettings.TestDevices.Add(SystemInfo.deviceUniqueIdentifier.ToUpper().Trim());
            }
            bool isInitialized = false;
            List<string> addList = new List<string>();
            foreach(var t in advertiser.Ads)
                addList.AddRange(t.ADIds);

            ColonyAd.Configure(advertiser.ID, appOptions, addList.ToArray());
            
            ColonyAd.OnConfigurationCompleted += (zones)=> 
            {
                isInitialized = true;
                Debug.LogError(zones.Count);
                if (zones == null || zones.Count <= 0)
                {
                    Debug.Log("Ads Status : AdColony Configure Failed");
                }
                else
                {
                    Debug.Log("Ads Status : AdColony Configure Success");

                    foreach (var t in advertiser.Ads)
                        t.Intialize(advertiser.advertiser);
                }
            };

            while (!isInitialized) yield return null;
            
        }
    }
}
#endif