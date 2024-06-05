#if Admob
using GoogleMobileAds.Api;
using GoogleMobileAds.Ump.Api;
using SH.Ads.Base;
using System;
using UnityEngine;

namespace SH.Ads.Admob
{
    public class Interstitial : BaseAdHandler
    {
        const string TestAdUnitId = "ca-app-pub-3940256099942544/1033173712";

        private InterstitialAd interstitial;

        protected internal override bool IsAdAvailable => IsIntialized && interstitial != null && interstitial.CanShowAd();
        protected internal override bool IsAdShowing { get; protected set; }

        internal override void Initialize(AD ad)
        {
            IDs = ad.ADIds;
            IsIntialized = true;
            adReshowTime = ad.ADReshowTime;
            loadAfterClose = ad.LoadAfterClose;

            if (ad.LoadAtStart)
                Load();

            Debug.Log($"{this} is initialized with {IDs.Count} ad Ids");
        }

        protected override void Load()
        {
            if (adLoading || !IsIntialized || !ConsentInformation.CanRequestAds())
                return;

            if (IDs.Count > 0)
            {
                string adUnitId = TestMode ? TestAdUnitId : IDs[count];
                AdRequest request = BuildAdRequest();

                InterstitialAd.Load(adUnitId, request, (loadedAd, error) =>
                {
                    if (error != null)
                    {
                        Debug.Log($"Ad log: {this} Failed to load: {error?.GetCode()}");
                        AdsManager.LogAnalyticEvent(this.ToString(), "On_Fail", error?.GetCode().ToString());
                        if (++count < IDs.Count)
                            Load();
                        else
                            count = 0;
                    }
                    else
                    {
                        interstitial = loadedAd;
                        interstitial.OnAdFullScreenContentClosed += OnAdClosed;
                        interstitial.OnAdFullScreenContentOpened += OnAdOpened;
                        interstitial.OnAdImpressionRecorded += OnAdImpressionRecorded;
                        interstitial.OnAdPaid += OnAdPaid;

                        Debug.Log($"Ad log: {this} Loaded");
                        AdsManager.LogAnalyticEvent(this.ToString(), "On_Load", count.ToString());
                    }
                });

                adLoading = true;
            }
        }

        internal override void Hide()
        {
            IsAdShowing = false;
        }

        internal override void Remove()
        {
            IsAdShowing = false;
        }

        internal override void Show()
        {
            if (IDs.Count == 0 || !CanAdBeShown)
                return;

            if (IsAdAvailable)
            {
                IsAdShowing = true;
                LastAdShownTime = DateTime.Now;
                interstitial.Show();
               
            }
            else
            {
                Load();
            }
        }

        private void OnAdClosed()
        {
            IsAdShowing = false;
            if (loadAfterClose)
                Load();
        }

        private void OnAdOpened()
        {
            IsAdShowing = true;
            AdsManager.LogAnalyticEvent(this.ToString(), "On_Show", count.ToString());
        }

        private void OnAdImpressionRecorded()
        {
            AdsManager.LogAnalyticEvent(this.ToString(), "On_Impression", count.ToString());
            Debug.Log($"Ad log: {this} impression recorded");
        }

        private void OnAdPaid(AdValue args)
        {
            AdsManager.LogAnalyticEvent(this.ToString(), "On_Paid", args.Value.ToString());
            Debug.Log($"Ad log: {this} ad paid: {args.Value}");
        }

        private AdRequest BuildAdRequest()
        {
            var tem = new AdRequest();
            tem.Extras.Add("npa", AdSettings.GDPRConcent ? "0" : "1");
            tem.Extras.Add("rdp", AdSettings.CCPAConsent ? "0" : "1");
            tem.Extras.Add("is_designed_for_families", AdSettings.IsForChildren ? "true" : "false");
            return tem;
        }
    }
}
#endif
