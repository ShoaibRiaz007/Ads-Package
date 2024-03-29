﻿#if AppLovin
using SH.Ads.Base;
using System;
using UnityEngine;

namespace SH.Ads.AppLovin
{
    public class RewardedInterstial : BaseAdHandler
    {
        protected internal override bool IsAdAvailable => IsIntialized && MaxSdk.IsRewardedInterstitialAdReady(IDs[count]);
        protected internal override bool IsAdShowing { get; protected set; }
        internal override void Intialize(AD ad)
        {
            IDs = ad.ADIds;
            IsIntialized = true;
            Debug.Log(this + " is intialized with " + IDs.Count + " ad Ids");
            loadAfterClose = ad.LoadAfterClose;

            MaxSdkCallbacks.RewardedInterstitial.OnAdLoadedEvent += OnAdLoadedEvent;
            MaxSdkCallbacks.RewardedInterstitial.OnAdHiddenEvent += OnAdCloseEvent;
            MaxSdkCallbacks.RewardedInterstitial.OnAdLoadFailedEvent += OnAdFailedEvent;
            MaxSdkCallbacks.RewardedInterstitial.OnAdClickedEvent += OnAdClickedEvent;
            MaxSdkCallbacks.RewardedInterstitial.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;

            if (ad.LoadAtStart)
                Load();
        }

        private void OnAdCloseEvent(string arg1, MaxSdkBase.AdInfo info)
        {
            IsAdShowing = false;
            AdsManager.OnRewardClosed?.Invoke();
            AdsManager.OnRewardClosed = null;
        }

        private void OnAdRevenuePaidEvent(string arg1, MaxSdkBase.AdInfo info)
        {
            Debug.Log($"Ad log : {this} Revenue : {info.RevenuePrecision} " + count);
            AdsManager.OnUserEarnedReward?.Invoke("Reward", AdSettings.RewardAmount);
            AdsManager.OnUserEarnedReward = null;
        }

        private void OnAdClickedEvent(string arg1, MaxSdkBase.AdInfo info)
        {
            Debug.Log($"Ad log : {this} Clicked :  " + count);
        }

        private void OnAdFailedEvent(string arg1, MaxSdkBase.ErrorInfo info)
        {
            Debug.Log($"Ad log : {this} Failed :  " + count);
            AdsManager.LogAnalyticEvent(this.ToString(), "On_Fail", info.AdLoadFailureInfo);
            if (count + 1 < IDs.Count)
            {
                count++;
                Load();
                return;
            }
            count = 0;
            IsAdShowing = false;
            adLoading = false;
        }

        private void OnAdLoadedEvent(string arg1, MaxSdkBase.AdInfo info)
        {
            adLoading = false;
            AdsManager.LogAnalyticEvent(this.ToString(), "On_Load", count.ToString());
        }

        protected override void Load()
        {
            if (adLoading || !IsIntialized)
                return;

            if (IDs.Count > 0)
            {
                Remove();
                MaxSdk.LoadRewardedInterstitialAd(IDs[count]);
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
            if (IDs.Count == 0)
                return;

            if (IsAdAvailable)
            {
                IsAdShowing = true;
                MaxSdk.ShowRewardedInterstitialAd(IDs[count]);
            }
            else
            {
                Load();
            }
        }


    }
}
#endif