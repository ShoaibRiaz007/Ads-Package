#if AdColony
using SH.Ads.Base;
using UnityEngine;

using ColonyAd = AdColony.Ads;
using AdView = AdColony.InterstitialAd;
using AdOption = AdColony.AdOptions;
using System;

namespace SH.Ads.AdColony
{
    public class Interstitial : BaseAdHandler
    {

        AdView adView;
        protected internal override bool IsAdAvailable => IsIntialized  && adView != null;
        protected internal override bool IsAdShowing { get; protected set; }
        internal override void Intialize(AD ad)
        {
            IDs = ad.ADIds;
            IsIntialized = true;
            Debug.Log(this + " is intialized with " + IDs.Count + " ad Ids");
            loadAfterClose = ad.LoadAfterClose;
            if (ad.LoadAtStart)
                Load();
            ColonyAd.OnExpiring += OnAdLoad;
        }
        protected override void Load()
        {
             if (adLoading || !IsIntialized)
                return;

            if ( IDs.Count > 0)
            {
                Remove();
                ColonyAd.OnRequestInterstitial += OnAdLoad;
                ColonyAd.OnRequestInterstitialFailed += OnAdFailedToLoad;
                ColonyAd.OnClosed += OnAdClosed;
                ColonyAd.RequestInterstitialAd(IDs[count], Option);
                adLoading = true;
            }
        }
        internal override void Hide()
        {
            if (IsAdAvailable)
                adView.DestroyAd();
            IsAdShowing = false;
        }
        internal override void Remove()
        {
            if (IsAdAvailable)
                adView.DestroyAd();

            IsAdShowing = false;
            ColonyAd.OnRequestInterstitial -= OnAdLoad;
            ColonyAd.OnRequestInterstitialFailed -= OnAdFailedToLoad;
            ColonyAd.OnClosed -= OnAdClosed;
        }
        internal override void Show()
        {
            if ( IDs.Count == 0)
                return;

            if (IsAdAvailable)
            {
                IsAdShowing = true;
                LocalAdShown = true;
                ColonyAd.ShowAd(adView);
            }
            else
                Load();

        }

        private void OnAdLoad(AdView view)
        {
            adLoading = false;
            adView = view;
            AdsManager.LogAnalyticEvent(this.ToString(), "On_Load", count.ToString());
        }

        private void OnAdFailedToLoad()
        {
            adLoading = false;
            Debug.Log($"Ad log : {this} Failed :  " + count);
            AdsManager.LogAnalyticEvent(this.ToString(), "On_Fail", count.ToString());
            adView = null;
            if (count + 1 <  IDs.Count)
            {
                count++;
                Load();
                return;
            }
            count = 0;
            return;
        }
        private void OnAdClosed(AdView ad)
        {
            adLoading=false;
            IsAdShowing = false;
            adView = null;
            if(loadAfterClose)
                Load();
        }


        AdOption Option
        {
            get
            {
                var tem = new AdOption();
                tem.ShowPostPopup = false;
                tem.ShowPrePopup = false;
                return tem;
            }
        }
    }
}
#endif