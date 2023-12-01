#if AppLovin
using SH.Ads.Base;
using System.Collections;

namespace SH.Ads.AppLovin
{
    /// <summary>
    /// Manager for Admob
    /// </summary>
    internal class Manager : BaseManager
    {
        internal override IEnumerator Initialize(Base.Advertiser advertiser, bool isForChildren, string ageGroup)
        {
            yield return IsNetworkAvailable();
            bool completed = false;
            MaxSdkCallbacks.OnSdkInitializedEvent += sdkConfiguration =>
            {
                MaxSdk.SetHasUserConsent(AdSettings.CCPAConsent);
                MaxSdk.SetDoNotSell(!AdSettings.GDPRConcent);
                MaxSdk.SetIsAgeRestrictedUser(AdSettings.IsForChildren);
                completed = true;
            };

            MaxSdk.SetSdkKey(advertiser.ID);
            MaxSdk.InitializeSdk();

            while (!completed)
                yield return null;

            foreach (var tem in advertiser.Ads)
                tem.Intialize(advertiser.advertiser);
        }
    }
    
}
#endif