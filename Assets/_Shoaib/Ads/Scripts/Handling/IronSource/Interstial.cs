#if IronSource
using SH.Ads.Base;
using UnityEngine;

using IronsourceAgent = IronSource;

namespace SH.Ads.IronSource
{
    public class Interstial : BaseAdHandler
    {
        protected internal override bool IsAdAvailable => IsIntialized && IronsourceAgent.Agent.isInterstitialReady();
        protected internal override bool IsAdShowing { get; protected set; }
        AdType adType;
        internal override void Intialize(AD ad)
        {
            IDs = ad.ADIds;
            IsIntialized = true;
            adType = ad.type;
            Debug.Log(this + " is intialized with " + IDs.Count + " ad Ids");
            loadAfterClose = ad.LoadAfterClose;


            IronSourceInterstitialEvents.onAdClosedEvent += (ad) =>
            {
                Debug.Log($"Ad log : {this} ad is closed : {count}");
                adLoading = false;
                IsAdShowing = false;
                if (loadAfterClose)
                    Load();
                AdsManager.LogAnalyticEvent(this.ToString(), "On_Load", count.ToString());
            };

            IronSourceInterstitialEvents.onAdReadyEvent += (ad) =>
            {
                Debug.Log($"Ad log : {this} ad is loaded : {count}");
                adLoading = false;
            };

            IronSourceInterstitialEvents.onAdShowSucceededEvent += (ad) =>
            {
                Debug.Log($"Ad log : {this} ad show to user successfully : {count}");
                adLoading = false;
                IsAdShowing = false;
            };
            IronSourceInterstitialEvents.onAdOpenedEvent += (ad) =>
            {
                Debug.Log($"Ad log : {this} ad is opened : {count}");
                adLoading = false;
                IsAdShowing = true;
            };

            IronSourceInterstitialEvents.onAdLoadFailedEvent += (ad) =>
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

                IronsourceAgent.Agent.loadInterstitial();
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
                IronsourceAgent.Agent.showInterstitial();
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