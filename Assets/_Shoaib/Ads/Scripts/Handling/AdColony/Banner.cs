#if AdColony
using SH.Ads.Base;
using UnityEngine;

using ColonyAd = AdColony.Ads;
using AdView = AdColony.AdColonyAdView;
using AdSize = AdColony.AdSize;
using AdPosition = AdColony.AdPosition;
using AdOption = AdColony.AdOptions;

namespace SH.Ads.AdColony
{
    public class Banner : BaseAdHandler
    {

        AdView adView;
        AdType adType; 
        protected internal override bool IsAdAvailable => IsIntialized  && adView != null;
        protected internal override bool IsAdShowing { get; protected set; }
        internal override void Intialize(AD ad)
        {
           IDs = ad.ADIds;
           adType = ad.type;
           IsIntialized = true;
            loadAfterClose = ad.LoadAfterClose;
            if (ad.LoadAtStart)
                Load();
            Debug.Log(this + " is intialized with " + IDs.Count + " ad Ids");
        }
        protected override void Load()
        {
            if (adLoading || !IsIntialized)
                return;

            if ( IDs.Count > 0)
            {
                Remove();
                ColonyAd.OnAdViewLoaded += OnAdLoad;
                ColonyAd.OnAdViewFailedToLoad += OnAdFailedToLoad;

                switch (adType)
                {
                    case AdType.Banner:
                        ColonyAd.RequestAdView(IDs[count], AdSize.Banner, AdPosition.Top, Option);
                        break;
                    case AdType.BigBanner:
                        ColonyAd.RequestAdView(IDs[count], AdSize.MediumRectangle, AdPosition.BottomRight, Option);
                        break;
                }
               
                adLoading = true;
            }
        }

        internal override void Hide()
        {
            if (IsAdAvailable)
                adView.HideAdView();
            IsAdShowing = false;
        }
        internal override void Remove()
        {
            if (IsAdAvailable)
                adView.DestroyAdView();
            IsAdShowing = false;
            ColonyAd.OnAdViewLoaded -= OnAdLoad;
            ColonyAd.OnAdViewFailedToLoad -= OnAdFailedToLoad;
        }
        internal override void Show()
        {
            if ( IDs.Count == 0)
                return;

            if (IsAdAvailable)
            {
                adView.ShowAdView();
                IsAdShowing = true;
            }
            else
                Load();

        }

        private void OnAdLoad(AdView view)
        {
            adLoading = false;
            adView = view;
            AdsManager.LogAnalyticEvent(this.ToString(), "On_Load", count.ToString());
        }

        private void OnAdFailedToLoad(AdView view)
        {
            adLoading = false;
            Debug.Log($"Ad log : {this} Failed :  " + count);
            AdsManager.LogAnalyticEvent(this.ToString(), "On_Fail",count.ToString());
            adView = null;
            if (count + 1 <  IDs.Count)
            {
                count++;
                Load();
                return;
            }
            count = 0;
            IsAdShowing = false;
        }


        AdOption Option
        {
            get
            {
                var tem = new AdOption();
                tem.ShowPostPopup = false;
                tem.ShowPrePopup = false;
                return tem;
            }
        }
    }
}
#endif