#if Admob
using GoogleMobileAds.Api;
using GoogleMobileAds.Ump.Api;
using SH.Ads.API.UI;
using SH.Ads.Base;
using System;
using UnityEngine;

namespace SH.Ads.Admob
{
    public class Rewarded : BaseAdHandler
    {
        const string _Test_ID = "ca-app-pub-3940256099942544/5224354917";

        private RewardedAd rewardedAd;
        private RewardPanel m_UIRewardPanel;

        protected internal override bool IsAdAvailable => IsIntialized && rewardedAd != null && rewardedAd.CanShowAd();
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
            m_UIRewardPanel = RewardPanel.Load(CanvasTransform);
        }

        protected override void Load()
        {
            if (adLoading || !IsIntialized || !ConsentInformation.CanRequestAds())
                return;

            if (IDs.Count > 0)
            {
                RewardedAd.Load(TestMode ? _Test_ID : IDs[count], AdRequestBuild, AdLoadCallBack);
                adLoading = true;
            }
        }

        internal override void Hide()
        {
            if (rewardedAd != null)
                rewardedAd.Destroy();
            IsAdShowing = false;
        }

        internal override void Remove()
        {
            if (rewardedAd != null)
                rewardedAd.Destroy();
            IsAdShowing = false;
        }

        internal override void Show()
        {
            if (IDs.Count == 0)
                return;
            if (!CanAdBeShown)
                return;

            if (IsAdAvailable)
            {
                m_UIRewardPanel.Show((value) =>
                {
                    if (!value)
                        return;

                    AdsManager.BGRunnerInstance.StartCoroutine(ShowRewardedPlaceholder(() =>
                    {
                        LocalAdShown = true;
                        IsAdShowing = true;
                        LastAdShownTime = DateTime.Now;
                        rewardedAd.Show(HandleUserEarnedReward);
                        AdsManager.LogAnalyticEvent(this.ToString(), "On_Show", count.ToString());
                    }));
                });
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

        private void AdLoadCallBack(RewardedAd ad, LoadAdError error)
        {
            if (error != null)
            {
                AdsManager.LogAnalyticEvent(this.ToString(), "On_Fail", error.GetCause().GetMessage());
                Debug.Log($"Ad log : {this} Failed : {count} cause : {error.GetCause()}");
                if (count + 1 < IDs.Count)
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
                rewardedAd = ad;
                rewardedAd.OnAdFullScreenContentClosed += () =>
                {
                    adLoading = false;
                    IsAdShowing = false;
                    if (loadAfterClose)
                        Load();
                    AdsManager.OnRewardClosed?.Invoke();
                    AdsManager.OnRewardClosed = null;
                };
                rewardedAd.OnAdImpressionRecorded += () =>
                {
                    AdsManager.LogAnalyticEvent(this.ToString(), "On_Impression", count.ToString());
                    Debug.Log($"Ad log : rewardedAd impression Recorded : {count}");
                };
                rewardedAd.OnAdPaid += (v) =>
                {
                    AdsManager.LogAnalyticEvent(this.ToString(), "On_Paid", v.Value.ToString());
                    Debug.Log($"Ad log : rewardedAd paid : {v.Value}");
                };
                AdsManager.LogAnalyticEvent(this.ToString(), "On_Load", count.ToString());
            }
        }

        private AdRequest AdRequestBuild
        {
            get
            {
                var request = new AdRequest();
                request.Extras.Add("npa", AdSettings.GDPRConcent ? "0" : "1");
                request.Extras.Add("rdp", AdSettings.CCPAConsent ? "0" : "1");
                request.Extras.Add("is_designed_for_families", AdSettings.IsForChildren ? "true" : "false");
                return request;
            }
        }
    }
}
#endif
