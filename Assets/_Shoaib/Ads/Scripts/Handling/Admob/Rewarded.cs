#if Admob
using GoogleMobileAds.Api;
using GoogleMobileAds.Ump.Api;
using SH.Ads.Base;
using UnityEngine;

namespace SH.Ads.Admob
{
    public class Rewarded : BaseAdHandler
    {
        const string _Test_ID = "ca-app-pub-3940256099942544/5224354917";


        RewardedAd rewardedAd;
        protected internal override bool IsAdAvailable => IsIntialized  && rewardedAd != null && rewardedAd.CanShowAd();
        internal override void Intialize(AD ad)
        {
            IDs = ad.adIds;
            IsIntialized = true;
            loadAfterClose = ad.loadAfterClose;
            if (ad.loadAtStart)
                Load();
            Debug.Log(this + " is intialized with " + IDs.Count + " ad Ids");
        }
        protected override void Load()
        {
            if (adLoading || !IsIntialized || !ConsentInformation.CanRequestAds())
                return;

            if ( IDs.Count > 0)
            {
                RewardedAd.Load(TestMode ? _Test_ID : IDs[count], AdRequestBuild, AdLoadCallBack);
                adLoading = true;
            }
        }
        internal override void Hide()
        {
            if (rewardedAd != null)
                rewardedAd.Destroy();
        }
        internal override void Remove()
        {
            if (rewardedAd != null)
                rewardedAd.Destroy();
        }
        internal override void Show()
        {
            if ( IDs.Count == 0)
                return;

            if (IsAdAvailable)
            {
                AdsManager.BGRunnerInstance.StartCoroutine(ShowRewardedPlaceholder(() => {
                    LocalAdShown = true;
                    rewardedAd.Show(HandleUserEarnedReward);
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

        private void AdLoadCallBack(RewardedAd ad, LoadAdError error)
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
                rewardedAd = ad;
                rewardedAd.OnAdFullScreenContentClosed += ()=>
                {
                    adLoading = false;
                    if (loadAfterClose)
                        Load();
                };
                rewardedAd.OnAdImpressionRecorded += () =>
                {
                    Debug.Log("Ad log : rewardedAd impression Recorded :  " + count);
                };
                rewardedAd.OnAdPaid += (v) =>
                {
                    Debug.Log("Ad log : rewardedAd paid :  " + v.Value);
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