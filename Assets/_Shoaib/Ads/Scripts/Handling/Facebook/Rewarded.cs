#if Facebook
using SH.Ads.Base;
using AudienceNetwork;
using UnityEngine;
using GoogleMobileAds.Api.AdManager;

namespace SH.Ads.Facebook
{
    public class Rewarded : BaseAdHandler
    {
        RewardedVideoAd adInstance;
        protected internal override bool IsAdAvailable => IsIntialized && adInstance != null && adInstance.IsValid();
        protected internal override bool IsAdShowing { get; protected set; }
        internal override void Intialize(AD ad)
        {
            IDs = ad.ADIds;
            IsIntialized = true;
            Debug.Log(this + " is intialized with " + IDs.Count + " ad Ids");
            loadAfterClose = ad.LoadAfterClose;
            if (ad.LoadAtStart)
                Load();
        }
        protected override void Load()
        {
            if (adLoading || !IsIntialized)
                return;

            if (IDs.Count > 0)
            {
                Remove();
                adInstance = new RewardedVideoAd(IDs[count]);
                adInstance.Register(new GameObject("Handler Facebook Rewarded"));

                adInstance.RewardedVideoAdDidLoad += () => { adLoading = false; AdsManager.LogAnalyticEvent(this.ToString(), "On_Load", count.ToString()); };
                adInstance.RewardedVideoAdDidFailWithError += (error) =>
                {
                    adLoading = false;
                    Debug.Log($"Ad log : {this} Failed :  {count} cause : {error}");
                    AdsManager.LogAnalyticEvent(this.ToString(), "On_Fail", error);
                    if (count + 1 < IDs.Count)
                    {
                        count++;
                        Load();
                        return;
                    }
                    count = 0;
                };
                adInstance.RewardedVideoAdWillLogImpression += () => { Debug.Log($"Ad log : {this} impression loged of ad id : {count}"); };
                adInstance.RewardedVideoAdDidClick += () => { Debug.Log($"Ad log : {this} click impression of ad id : {count}"); };
                adInstance.RewardedVideoAdDidClose += () =>
                {
                    Debug.Log($"Ad log : {this} close impression of ad id : {count}");
                    IsAdShowing = false;
                    if (loadAfterClose)
                        Load();
                    AdsManager.OnRewardClosed?.Invoke();
                    AdsManager.OnRewardClosed = null;
                };
                adInstance.rewardedVideoAdComplete += () =>
                {
                    AdsManager.OnUserEarnedReward?.Invoke("Reward",AdSettings.RewardAmount);
                    AdsManager.OnUserEarnedReward = null;
                };
#if UNITY_ANDROID
                /*
                 * Only relevant to Android.
                 * This callback will only be triggered if the Interstitial activity has
                 * been destroyed without being properly closed. This can happen if an
                 * app with launchMode:singleTask (such as a Unity game) goes to
                 * background and is then relaunched by tapping the icon.
                 */
                adInstance.rewardedVideoAdActivityDestroyed = () =>
                {
                    IsAdShowing = false;
                    Remove();
                };
#endif
                adLoading = true;
            }
        }

        internal override void Hide()
        {
            if (adInstance != null)
                adInstance.Dispose();
            IsAdShowing = false;
        }
        internal override void Remove()
        {
            if (adInstance != null)
                adInstance.Dispose();
            IsAdShowing = false;
        }
        internal override void Show()
        {
            if (IDs.Count == 0)
                return;
            if (IsAdAvailable)
            {
                AdsManager.BGRunnerInstance.StartCoroutine(ShowRewardedPlaceholder(() =>
                {
                    IsAdShowing = true;
                    LocalAdShown = true;
                    adInstance.Show();
                }));
            }
            else
            {
                Load();
            }
        }


    }
}
#endif