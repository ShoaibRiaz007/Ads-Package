#if IronSource
using SH.Ads.Base;
using UnityEngine;

using IronsourceAgent = IronSource;

namespace SH.Ads.IronSource
{
    public class Banner : BaseAdHandler
    {
        protected internal override bool IsAdAvailable => IsIntialized && adUnit!=null;
        IronSourceAdInfo adUnit;
        AdType adType;
        internal override void Intialize(AD ad)
        {
            IDs = ad.adIds;
            IsIntialized = true;
            adType = ad.adType;
            Debug.Log(this + " is intialized with " + IDs.Count + " ad Ids");
            loadAfterClose = ad.loadAfterClose;


            IronSourceBannerEvents.onAdLoadedEvent += (ad) =>
            {
                Debug.Log($"Ad log : {this} ad is loaded ad displayed : {count}");
                adUnit = ad;
                adLoading = false;
            };

            IronSourceBannerEvents.onAdLoadFailedEvent += (ad) =>
            {
                Debug.Log($"Ad log : {this} ad failled to load : {count}");
                adLoading = false;
                if (count + 1 < IDs.Count)
                {
                    count++;
                    Load();
                    return;
                }
                count = 0;
            };

            IronSourceBannerEvents.onAdClickedEvent += (ad) =>
            {
                Debug.Log($"Ad log : {this} ad is clicked: {count}");

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
                IronSourceBannerSize bannerSize = IronSourceBannerSize.BANNER;
                IronSourceBannerPosition bannerPosition = IronSourceBannerPosition.TOP;
                switch (adType)
                {
                    case AdType.Banner:
                        bannerSize = IronSourceBannerSize.SMART;
                        bannerPosition = IronSourceBannerPosition.TOP;
                        break;
                    case AdType.BigBanner:
                        bannerSize = IronSourceBannerSize.RECTANGLE;
                        bannerPosition = IronSourceBannerPosition.BOTTOM;
                        break;
                }

                IronsourceAgent.Agent.loadBanner(bannerSize, bannerPosition, IDs[count]);
                adLoading = true;
            }
        }

        internal override void Hide()
        {
            if(adUnit!=null)
                IronsourceAgent.Agent.hideBanner();
        }
        internal override void Remove()
        {
            if (adUnit != null)
                IronsourceAgent.Agent.destroyBanner();
        }
        internal override void Show()
        {
            if (IDs.Count == 0)
                return;
            if (IsAdAvailable)
            {
                IronsourceAgent.Agent.displayBanner();
            }
            else
            {
                Load();
            }
        }


    }
}
#endif