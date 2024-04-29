#if AppLovin
using SH.Ads.Base;
using UnityEngine;

namespace SH.Ads.AppLovin
{
    public class Banner : BaseAdHandler
    {
        protected internal override bool IsAdAvailable => IsIntialized && loaded && !adLoading;
        protected internal override bool IsAdShowing { get; protected set; }
        AdType adType;
        bool loaded = false;
        internal override void Intialize(AD ad)
        {
            IDs = ad.ADIds;
            IsIntialized = true;
            adType = ad.type;
            Debug.Log(this + " is intialized with " + IDs.Count + " ad Ids");
            loadAfterClose = ad.LoadAfterClose;

            MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnBannerAdLoadedEvent;
            MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnBannerAdFailedEvent;
            MaxSdkCallbacks.Banner.OnAdClickedEvent += OnBannerAdClickedEvent;
            MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnBannerAdRevenuePaidEvent;

            if (ad.LoadAtStart)
                Load();
        }

        private void OnBannerAdRevenuePaidEvent(string arg1, MaxSdkBase.AdInfo info)
        {
            Debug.Log($"Ad log : {this} Revenue : {info.RevenuePrecision} " + count);
        }

        private void OnBannerAdClickedEvent(string arg1, MaxSdkBase.AdInfo info)
        {
            Debug.Log($"Ad log : {this} Clicked :  " + count);
        }

        private void OnBannerAdFailedEvent(string arg1, MaxSdkBase.ErrorInfo info)
        {
            Debug.Log($"Ad log : {this} Failed :  " + count);
            AdsManager.LogAnalyticEvent(this.ToString(), "On_Fail",info.AdLoadFailureInfo);
            if (count + 1 < IDs.Count)
            {
                count++;
                Load();
                return;
            }
            count = 0;
            IsAdShowing = false;
            adLoading = false;
            loaded=false;
        }

        private void OnBannerAdLoadedEvent(string arg1, MaxSdkBase.AdInfo info)
        {
            adLoading = false;
            loaded = true;
            AdsManager.LogAnalyticEvent(this.ToString(), "On_Load", count.ToString());
        }

        protected override void Load()
        {
            if (adLoading || !IsIntialized)
                return;

            if (IDs.Count > 0)
            {
                Remove();
               
                if (adType == AdType.Banner)
                {
                    MaxSdk.CreateBanner(IDs[count], MaxSdkBase.BannerPosition.TopCenter);
                    MaxSdk.SetBannerBackgroundColor(IDs[count], Color.black);
                    adLoading = true;
                }
               
            }
        }

        internal override void Hide()
        {
            MaxSdk.HideBanner(IDs[count]);
            IsAdShowing = false;
        }
        internal override void Remove()
        {
            MaxSdk.DestroyBanner(IDs[count]);
            IsAdShowing = false;
            loaded= false;
        }
        internal override void Show()
        {
            if (IDs.Count == 0)
                return;
            if (IsAdAvailable)
            {
                IsAdShowing = true;
                MaxSdk.ShowBanner(IDs[count]);
            }
            else
            {
                Load();
            }
        }


    }
}
#endif