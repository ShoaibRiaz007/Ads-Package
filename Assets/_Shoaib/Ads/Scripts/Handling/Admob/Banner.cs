#if Admob
using GoogleMobileAds.Api;
using GoogleMobileAds.Ump.Api;
using SH.Ads.Base;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace SH.Ads.Admob
{
    public class Banner : BaseAdHandler
    {
        const string TestAdUnitId = "ca-app-pub-3940256099942544/6300978111";

        private BannerView bannerView;
        private AdType adType;
        private GameObject placeHolder;

        protected internal override bool IsAdAvailable => IsIntialized && bannerView != null && !bannerView.IsDestroyed;
        protected internal override bool IsAdShowing { get; protected set; }

        internal override void Initialize(AD ad)
        {
            IDs = ad.ADIds;
            adType = ad.type;
            adReshowTime = ad.ADReshowTime;
            IsIntialized = true;
            loadAfterClose = ad.LoadAfterClose;
            CreatePlaceHolder();
            if (ad.LoadAtStart)
                Load();

            Debug.Log($"{this} is initialized with {IDs.Count} ad Ids");
        }

        protected override void Load()
        {
            if (adLoading || !IsIntialized || !ConsentInformation.CanRequestAds())
                return;

            if (IDs.Count > 0)
            {
                Remove();
                string adUnitId = TestMode ? TestAdUnitId : IDs[count];
                AdSize adSize = (adType == AdType.Banner) ? AdSize.Banner : AdSize.MediumRectangle;
                bannerView = new BannerView(adUnitId, adSize, (adType == AdType.Banner)?AdPosition.Top : AdPosition.BottomLeft);
                
                bannerView.OnBannerAdLoadFailed += OnAdFailedToLoad;
                bannerView.OnBannerAdLoaded += OnAdLoaded;
                bannerView.OnAdImpressionRecorded += OnAdImpressionRecorded;
                bannerView.OnAdPaid += OnAdPaid;

                bannerView.LoadAd(AdRequestBuild);
                adLoading = true;
            }
        }

        internal override void Hide()
        {
            bannerView?.Hide();
            IsAdShowing = false;
            placeHolder?.SetActive(false);
        }

        internal override void Remove()
        {
            if (bannerView != null)
            {
                bannerView.OnBannerAdLoadFailed -= OnAdFailedToLoad;
                bannerView.OnBannerAdLoaded -= OnAdLoaded;
                bannerView.OnAdImpressionRecorded -= OnAdImpressionRecorded;
                bannerView.OnAdPaid -= OnAdPaid;
                bannerView.Destroy();
            }
            IsAdShowing = false;
            placeHolder?.SetActive(false);
        }

        internal override void Show()
        {
            if (IDs.Count == 0 || !CanAdBeShown)
                return;

            if (IsAdAvailable)
            {
                IsAdShowing = true;
                LastAdShownTime = DateTime.Now;
                bannerView.Show();
            }
            else
                Load();
        }

        private void OnAdFailedToLoad(LoadAdError error)
        {
            Debug.Log($"Ad log: {this} Failed to load: {error?.GetCode()}");
            AdsManager.LogAnalyticEvent(this.ToString(), "On_Fail", error?.GetCode().ToString());
            placeHolder?.SetActive(false);
            bannerView = null;
            adLoading = false;
            if (++count < IDs.Count)
                Load();
            else
                count = 0;
        }

        private void OnAdLoaded()
        {
            Debug.Log("Ad log: Banner loaded");
            AdsManager.LogAnalyticEvent(this.ToString(), "On_Load", count.ToString());
            placeHolder?.SetActive(true);
            adLoading = false;
            IsAdShowing = true;
            bannerView.Show();
        }

        private void OnAdImpressionRecorded()
        {
            AdsManager.LogAnalyticEvent(this.ToString(), "On_Impression", count.ToString());
            Debug.Log("Ad log: Banner impression recorded");
        }

        private void OnAdPaid(AdValue adValue)
        {
            AdsManager.LogAnalyticEvent(this.ToString(), "On_OnPaid", adValue.Value.ToString());
            Debug.Log($"Ad log: Banner ad paid: {adValue.Value}");
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

        private void CreatePlaceHolder()
        {
            placeHolder = new GameObject("Banner Placeholder", typeof(RectTransform));
            placeHolder.transform.SetParent(CanvasTransform.GetChild(0));
            var bannerRect = placeHolder.GetComponent<RectTransform>();
            float width = 0, height = 0;
            switch (adType)
            {
                case AdType.Banner:
                    width = AdSize.Banner.Width + 10;
                    height = AdSize.Banner.Height + 10;
                    bannerRect.sizeDelta = new Vector2(width, height);
                    bannerRect.anchorMin = new Vector2(0.5f, 1f);
                    bannerRect.anchorMax = new Vector2(0.5f, 1f);
                    bannerRect.anchoredPosition = new Vector2(0, -height / 2);
                    break;
                case AdType.BigBanner:
                    width = AdSize.MediumRectangle.Width * Screen.dpi / 160 + 10;
                    height = AdSize.MediumRectangle.Height * Screen.dpi / 160 + 20;
                    bannerRect.sizeDelta = new Vector2(width, height);
                    bannerRect.anchorMin = new Vector2(0f, 0f);
                    bannerRect.anchorMax = new Vector2(0f, 0f);
                    bannerRect.anchoredPosition = new Vector2(width / 2, height / 2);
                    break;
            }
            placeHolder.gameObject.AddComponent<Image>().color = Color.black;
            placeHolder.SetActive(false);
        }
    }
}
#endif
