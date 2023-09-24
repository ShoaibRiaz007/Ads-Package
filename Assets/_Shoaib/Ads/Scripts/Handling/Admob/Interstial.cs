#if Admob
using GoogleMobileAds.Api;
using SH.Ads.Base;
using UnityEngine;

namespace SH.Ads.Admob
{
    public class Interstial : BaseAdHandler
    {
        const string _Test_ID = "ca-app-pub-3940256099942544/1033173712";


        InterstitialAd interstitial;
        protected internal override bool IsAdAvailable => IsIntialized  && interstitial != null && interstitial.CanShowAd();
        internal override void Intialize(AD ad)
        {
            IDs = ad.adIds;
            IsIntialized = true;

            Debug.Log(this + " is intialized with " + IDs.Count + " ad Ids");
            loadAfterClose = ad.loadAfterClose;
            if (ad.loadAtStart)
                Load();
        }
        protected override void Load()
        {
             if (adLoading || !IsIntialized)
                return;

            if ( IDs.Count > 0)
            {
                InterstitialAd.Load(TestMode ? _Test_ID : IDs[count], AdRequestBuild, CallBack);
                adLoading = true;
            }
        }
        internal override void Hide()
        {
            if (interstitial != null)
                interstitial.Destroy();
        }
        internal override void Remove()
        {
            if (interstitial != null)
                interstitial.Destroy();
        }
        internal override void Show()
        {
            if ( IDs.Count == 0)
                return;
            if (IsAdAvailable)
            {
                LocalAdShown = true;
                interstitial.Show();
            }
            else
            {
               Load();
            }
        }

        void CallBack(InterstitialAd ad, LoadAdError error)
        {

            if (error != null)
            {
                Debug.Log($"Ad log : {this} Failed :  {count} cause : {error.GetCause()}");
                if (count + 1 <  IDs.Count)
                {
                    count++;
                    adLoading = false;
                    Load();
                    return;
                }
                count = 0;
            }
            else
            {
                interstitial = ad;
                interstitial.OnAdFullScreenContentClosed += ()=> 
                {
                    adLoading = false;
                    if (loadAfterClose)
                        Load();
                };
                interstitial.OnAdImpressionRecorded += () =>
                {
                    Debug.Log($"Ad log : Interstial impression Recorded :  " + count);
                };
                interstitial.OnAdPaid += (v) =>
                {
                    Debug.Log("Ad log : Interstial ad paid :  " + v.Value);
                };
            }
        }

        AdRequest AdRequestBuild
        {
            get
            {
                var tem = new AdRequest();
                tem.Extras.Add("npa", AdSettings.GDPRConcent ? "0" : "1");
                tem.Extras.Add("rdp", AdSettings.CCPAConsent ? "0" : "1");
                tem.Extras.Add("is_designed_for_families", AdSettings.IsForChildren ? "true" : "false");
                return tem;
            }
        }
    }
}
#endif