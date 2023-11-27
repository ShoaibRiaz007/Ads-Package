#if Admob
using GoogleMobileAds.Api;
using GoogleMobileAds.Ump.Api;
using SH.Ads.Base;
using System;
using UnityEngine;

namespace SH.Ads.Admob
{
    public class Interstial : BaseAdHandler
    {
        const string _Test_ID = "ca-app-pub-3940256099942544/1033173712";


        InterstitialAd interstitial;
        protected internal override bool IsAdAvailable => IsIntialized  && interstitial != null && interstitial.CanShowAd();
        protected internal override bool IsAdShowing { get; protected set; }
        internal override void Intialize(AD ad)
        {
            IDs = ad.ADIds;
            IsIntialized = true;
            adReshowTime = ad.ADReshowTime;
            Debug.Log(this + " is intialized with " + IDs.Count + " ad Ids");
            loadAfterClose = ad.LoadAfterClose;
            if (ad.LoadAtStart)
                Load();
        }
        protected override void Load()
        {
            if (adLoading || !IsIntialized || !ConsentInformation.CanRequestAds())
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

            IsAdShowing = false;
        }
        internal override void Remove()
        {
            if (interstitial != null)
                interstitial.Destroy();

            IsAdShowing = false;
        }
        internal override void Show()
        {
            if ( IDs.Count == 0)
                return;

            if (!CanAdBeShown)
                return;
            

            if (IsAdAvailable)
            {
                IsAdShowing = true;
                LocalAdShown = true;
                interstitial.Show();
                LastAdShownTime = DateTime.Now;
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
                IsAdShowing = false;
                count = 0;
            }
            else
            {
                interstitial = ad;
                interstitial.OnAdFullScreenContentClosed += ()=> 
                {
                    adLoading = false;
                    IsAdShowing = false;
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