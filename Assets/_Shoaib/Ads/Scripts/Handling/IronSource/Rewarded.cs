#if IronSource
using SH.Ads.Base;
using UnityEngine;

using IronsourceAgent = IronSource;

namespace SH.Ads.IronSource
{
    public class Rewarded : BaseAdHandler
    {
        protected internal override bool IsAdAvailable => IsIntialized && IronsourceAgent.Agent.isRewardedVideoAvailable();
        AdType adType;
        internal override void Intialize(AD ad)
        {
            IDs = ad.adIds;
            IsIntialized = true;
            adType = ad.adType;
            Debug.Log(this + " is intialized with " + IDs.Count + " ad Ids");
            loadAfterClose = ad.loadAfterClose;


            IronSourceRewardedVideoEvents.onAdClosedEvent += (ad) =>
            {
                Debug.Log($"Ad log : {this} ad is closed : {count}");
                if (loadAfterClose)
                    Load();
            };

            IronSourceRewardedVideoEvents.onAdReadyEvent += (ad) =>
            {
                Debug.Log($"Ad log : {this} ad is loaded : {count}");
                adLoading = false;
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
            };

            IronSourceRewardedVideoEvents.onAdLoadFailedEvent += (ad) =>
            {
                Debug.Log($"Ad log : {this} ad is closed with error : [{ad.getErrorCode()}] at index: {count}");
                adLoading = false;
                if (count + 1 < IDs.Count)
                {
                    count++;
                    Load();
                    return;
                }
                count = 0;
            };


            if (ad.loadAtStart)
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
        }
        internal override void Remove()
        {
        }
        internal override void Show()
        {
            if (IDs.Count == 0)
                return;
            if (IsAdAvailable)
            {
                IronsourceAgent.Agent.showRewardedVideo(IDs[count]);
                LocalAdShown = true;
            }
            else
            {
                Load();
            }
        }


    }
}
#endif