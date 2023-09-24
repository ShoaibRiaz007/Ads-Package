#if Admob
using GoogleMobileAds.Api;
using SH.Ads.Base;
using UnityEngine;

namespace SH.Ads.Admob
{
    public class RewardedInterstial : BaseAdHandler
    {
        const string _Test_ID = "ca-app-pub-3940256099942544/5354046379";


        RewardedInterstitialAd interStialReward;
  
        protected internal override bool IsAdAvailable => IsIntialized  && interStialReward != null && interStialReward.CanShowAd();
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
                RewardedInterstitialAd.Load(TestMode ? _Test_ID : IDs[count], AdRequestBuild, Callback);
                adLoading = true;
            }
        }

        internal override void Hide()
        {
            if (interStialReward != null)
                interStialReward.Destroy();
        }
        internal override void Remove()
        {
            if (interStialReward != null)
                interStialReward.Destroy();
        }
        internal override void Show()
        {
            if ( IDs.Count == 0)
                return;

            if (IsAdAvailable)
            {
                AdsManager.BGRunnerInstance.StartCoroutine(ShowRewardedPlaceholder(() =>
                {
                    LocalAdShown = true;
                    interStialReward.Show(HandleUserEarnedReward);
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
                    if(loadAfterClose)
                        Load();
                };
                interStialReward.OnAdImpressionRecorded += () =>
                {
                    Debug.Log("Ad log : Interstial Rewarded impression Recorded :  " + count);
                };
                interStialReward.OnAdPaid += (v) =>
                {
                    Debug.Log("Ad log : Interstial rewarded paid :  " + v.Value);
                };
            }
            else
            {
                Debug.Log($"Ad log : {this} Failed :  {count} cause : {error.GetCause()}");
                interStialReward = null;
                if (count + 1 <  IDs.Count)
                {
                    count++;
                    adLoading = false;
                    Load();
                    return;
                }
                
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