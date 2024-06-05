#if Admob
using GoogleMobileAds.Api;
using GoogleMobileAds.Ump.Api;
using SH.Ads.Base;
using System;
using UnityEngine;

namespace SH.Ads.Admob
{
    public class RewardedInterstitial : BaseAdHandler
    {
        const string _Test_ID = "ca-app-pub-3940256099942544/5354046379";


        RewardedInterstitialAd interStialReward;
  
        protected internal override bool IsAdAvailable => IsIntialized  && interStialReward != null && interStialReward.CanShowAd();
        protected internal override bool IsAdShowing { get; protected set; }
        internal override void Initialize(AD ad)
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
                RewardedInterstitialAd.Load(TestMode ? _Test_ID : IDs[count], AdRequestBuild, Callback);
                adLoading = true;
            }
        }

        internal override void Hide()
        {
            if (interStialReward != null)
                interStialReward.Destroy();
            IsAdShowing = false;
        }
        internal override void Remove()
        {
            if (interStialReward != null)
                interStialReward.Destroy();
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
                AdsManager.BGRunnerInstance.StartCoroutine(ShowRewardedPlaceholder(() =>
                {
                    IsAdShowing = true;
                    LocalAdShown = true;
                    LastAdShownTime = DateTime.Now;
                    interStialReward.Show(HandleUserEarnedReward);
                    AdsManager.LogAnalyticEvent(this.ToString(), "On_Show", count.ToString());
                }));
            }
            else
            {
               Load();
            }
        }
        public void HandleUserEarnedReward(Reward reward)
        {
            AdsManager.OnUserEarnedReward?.Invoke(reward.Type, AdSettings.RewardAmount);
            AdsManager.OnUserEarnedReward = null;
        }
        void Callback(RewardedInterstitialAd ad, LoadAdError error)
        {
            if (error == null)
            {
                interStialReward = ad;
                interStialReward.OnAdFullScreenContentClosed += ()=> 
                {
                    adLoading = false;
                    IsAdShowing = false;
                    if (loadAfterClose)
                        Load();
                    AdsManager.OnRewardClosed?.Invoke();
                    AdsManager.OnRewardClosed = null;
                };
                interStialReward.OnAdImpressionRecorded += () =>
                {
                    AdsManager.LogAnalyticEvent(this.ToString(), "On_Impression", count.ToString());
                    Debug.Log("Ad log : Interstial Rewarded impression Recorded :  " + count);
                };
                interStialReward.OnAdPaid += (v) =>
                {
                    AdsManager.LogAnalyticEvent(this.ToString(), "On_Fail", v.Value.ToString());
                    Debug.Log("Ad log : Interstial rewarded paid :  " + v.Value);
                };
                AdsManager.LogAnalyticEvent(this.ToString(), "On_Load", count.ToString());
            }
            else
            {
                Debug.Log($"Ad log : {this} Failed :  {count} cause : {error.GetCause()}");
                AdsManager.LogAnalyticEvent(this.ToString(), "On_Fail", error.GetCause().GetMessage());
                interStialReward = null;
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