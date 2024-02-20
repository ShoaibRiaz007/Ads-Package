#if IronSource
using SH.Ads.Base;
using UnityEngine;

using IronsourceAgent = IronSource;

namespace SH.Ads.IronSource
{
    public class Rewarded : BaseAdHandler
    {
        protected internal override bool IsAdAvailable => IsIntialized && IronsourceAgent.Agent.isRewardedVideoAvailable();
        protected internal override bool IsAdShowing { get; protected set; }

        internal override void Intialize(AD ad)
        {
            IDs = ad.ADIds;
            IsIntialized = true;
            Debug.Log(this + " is intialized with " + IDs.Count + " ad Ids");
            loadAfterClose = ad.LoadAfterClose;


            IronSourceRewardedVideoEvents.onAdClosedEvent += (ad) =>
            {
                Debug.Log($"Ad log : {this} ad is closed : {count}");
                IsAdShowing = false;
                if (loadAfterClose)
                    Load();
                AdsManager.OnRewardClosed?.Invoke();
                AdsManager.OnRewardClosed = null;
               
            };

            IronSourceRewardedVideoEvents.onAdReadyEvent += (ad) =>
            {
                Debug.Log($"Ad log : {this} ad is loaded : {count}");
                adLoading = false;
                AdsManager.LogAnalyticEvent(this.ToString(), "On_Load", count.ToString());
            };

            IronSourceRewardedVideoEvents.onAdRewardedEvent += (placment,reward) =>
            {
                Debug.Log($"Ad log : {this} on user reward : {count}");
                AdsManager.OnUserEarnedReward?.Invoke("Reward", AdSettings.RewardAmount);
                AdsManager.OnUserEarnedReward = null;
            };
            IronSourceRewardedVideoEvents.onAdOpenedEvent += (ad) =>
            {
                Debug.Log($"Ad log : {this} ad is opened : {count}");
                IsAdShowing = true;
            };

            IronSourceRewardedVideoEvents.onAdLoadFailedEvent += (ad) =>
            {
                Debug.Log($"Ad log : {this} ad is closed with error : [{ad.getErrorCode()}] at index: {count}");
                AdsManager.LogAnalyticEvent(this.ToString(), "On_Fail", ad.getCode().ToString());
                adLoading = false;
                if (count + 1 < IDs.Count)
                {
                    count++;
                    Load();
                    return;
                }
                count = 0;
                IsAdShowing = false;
            };


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

                IronsourceAgent.Agent.loadRewardedVideo();
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
            if (IDs.Count == 0)
                return;
            if (IsAdAvailable)
            {
                IronsourceAgent.Agent.showRewardedVideo(IDs[count]);
                LocalAdShown = true;
                IsAdShowing = true;
            }
            else
            {
                Load();
            }
        }


    }
}
#endif