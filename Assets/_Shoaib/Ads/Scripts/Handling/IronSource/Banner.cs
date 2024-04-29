#if IronSource
using SH.Ads.Base;
using UnityEditor.PackageManager;
using UnityEngine;

using IronsourceAgent = IronSource;

namespace SH.Ads.IronSource
{
    public class Banner : BaseAdHandler
    {
        protected internal override bool IsAdAvailable => IsIntialized && adUnit!=null;
        protected internal override bool IsAdShowing { get; protected set; }
        IronSourceAdInfo adUnit;
        AdType adType;
        internal override void Intialize(AD ad)
        {
            IDs = ad.ADIds;
            IsIntialized = true;
            adType = ad.type;
            Debug.Log(this + " is intialized with " + IDs.Count + " ad Ids");
            loadAfterClose = ad.LoadAfterClose;


            IronSourceBannerEvents.onAdLoadedEvent += (ad) =>
            {
                Debug.Log($"Ad log : {this} ad is loaded ad displayed : {count}");
                adUnit = ad;
                adLoading = false;
                AdsManager.LogAnalyticEvent(this.ToString(), "On_Load", count.ToString());
            };

            IronSourceBannerEvents.onAdLoadFailedEvent += (ad) =>
            {
                Debug.Log($"Ad log : {this} ad failled to load : {count}");
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

            IronSourceBannerEvents.onAdClickedEvent += (ad) =>
            {
                Debug.Log($"Ad log : {this} ad is clicked: {count}");

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
            IsAdShowing = false;
        }
        internal override void Remove()
        {
            if (adUnit != null)
                IronsourceAgent.Agent.destroyBanner();
            IsAdShowing = false;
        }
        internal override void Show()
        {
            if (IDs.Count == 0)
                return;
            if (IsAdAvailable)
            {
                IsAdShowing = true;
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