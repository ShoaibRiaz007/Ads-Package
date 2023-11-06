#if IronSource
using SH.Ads.Base;
using UnityEngine;

using IronsourceAgent = IronSource;

namespace SH.Ads.IronSource
{
    public class Interstial : BaseAdHandler
    {
        protected internal override bool IsAdAvailable => IsIntialized && IronsourceAgent.Agent.isInterstitialReady();
        AdType adType;
        internal override void Intialize(AD ad)
        {
            IDs = ad.adIds;
            IsIntialized = true;
            adType = ad.adType;
            Debug.Log(this + " is intialized with " + IDs.Count + " ad Ids");
            loadAfterClose = ad.loadAfterClose;


            IronSourceInterstitialEvents.onAdClosedEvent += (ad) =>
            {
                Debug.Log($"Ad log : {this} ad is closed : {count}");
                adLoading = false;
                if (loadAfterClose)
                    Load();
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
            };
            IronSourceInterstitialEvents.onAdOpenedEvent += (ad) =>
            {
                Debug.Log($"Ad log : {this} ad is opened : {count}");
                adLoading = false;
            };

            IronSourceInterstitialEvents.onAdLoadFailedEvent += (ad) =>
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

                IronsourceAgent.Agent.loadInterstitial();
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
                IronsourceAgent.Agent.showInterstitial();
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