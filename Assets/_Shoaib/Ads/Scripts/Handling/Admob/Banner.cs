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
        const string _Test_ID = "ca-app-pub-3940256099942544/6300978111";
        
        private  BannerView bannerView;
        AdType adType;
        GameObject placeHolder;
        protected internal override bool IsAdAvailable => IsIntialized  && bannerView!=null;
        protected internal override bool IsAdShowing { get; protected set; }
        internal override void Intialize(AD ad)
        {
           
            IDs = ad.ADIds;
            adType = ad.type;
            adReshowTime = ad.ADReshowTime;
            IsIntialized = true;
            loadAfterClose = ad.LoadAfterClose;
            CreatePlaceHolder();
            if (ad.LoadAtStart)
                Load();

            Debug.Log(this + " is intialized with " + IDs.Count + " ad Ids");
        }
        protected override void Load()
        {
            if (adLoading || !IsIntialized || !ConsentInformation.CanRequestAds())
                return;

            if ( IDs.Count > 0)
            {
                Remove();
                switch (adType)
                {
                    case AdType.Banner:
                        bannerView = new BannerView(TestMode ? _Test_ID : IDs[count],AdSize.Banner, AdPosition.Top);
                        break;
                    case AdType.BigBanner:
                        bannerView = new BannerView(TestMode ? _Test_ID : IDs[count], AdSize.MediumRectangle, AdPosition.BottomLeft);
                        break;
                }
                bannerView.OnBannerAdLoadFailed += (arg)=> 
                {
                    Debug.Log($"Ad log : {this} Failed :  {count} cause : {arg.GetCause()}");
                    AdsManager.LogAnalyticEvent(this.ToString(), "On_Fail", arg.GetCause().GetMessage());
                    placeHolder.gameObject.SetActive(false);
                    bannerView = null;
                    adLoading = false;
                    if (count + 1 <  IDs.Count)
                    {
                        count++;
                        Load();
                        return;
                    }
                    count = 0;
                    return;
                };
                bannerView.OnBannerAdLoaded += () => 
                {
                    Debug.Log("Ad log : Banner loaded :  " + count);
                    AdsManager.LogAnalyticEvent(this.ToString(), "On_Load", count.ToString());
                    placeHolder?.gameObject.SetActive(true);
                    adLoading = false;
                    IsAdShowing = true;
                    bannerView.Show();
                };
                bannerView.OnAdImpressionRecorded += () =>
                {
                    AdsManager.LogAnalyticEvent(this.ToString(), "On_Impression", count.ToString());
                    Debug.Log("Ad log : Banner impression Recorded :  " + count);
                };
                bannerView.OnAdPaid += (v) =>
                {
                    AdsManager.LogAnalyticEvent(this.ToString(), "On_OnPaid", v.Value.ToString());
                    Debug.Log("Ad log : Banner ad paid :  " + v.Value);
                };
               
                bannerView.LoadAd(AdRequestBuild);
                adLoading = true;
            }
        }
        internal override void Hide()
        {
            if (IsAdAvailable)
                bannerView.Hide();
            IsAdShowing = false;
            placeHolder?.gameObject.SetActive(false);
        }
        internal override void Remove()
        {
            if (IsAdAvailable)
                bannerView.Destroy();
            IsAdShowing = false;
            placeHolder?.gameObject.SetActive(false);
        }
        internal override void Show()
        {
            if ( IDs.Count == 0)
                return;
            if (!CanAdBeShown)
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


        AdRequest AdRequestBuild
        {
            get
            {
                var tem = new AdRequest();
                tem.Extras.Add("npa", AdSettings.GDPRConcent?"0":"1");
                tem.Extras.Add("rdp", AdSettings.CCPAConsent?"0":"1");
                tem.Extras.Add("is_designed_for_families", AdSettings.IsForChildren?"true":"false");
                return tem;
            }
        }

        void CreatePlaceHolder()
        {
            placeHolder = new GameObject("Banner Placeholder",typeof(RectTransform));
            placeHolder.transform.parent = CanvasTransform.GetChild(0);
            var banner = placeHolder.GetComponent<RectTransform>();
            float width=0, height=0;
            switch (adType)
            {
                case AdType.Banner:
                    width = AdSize.Banner.Width+10;
                    height =AdSize.Banner.Height+10;
                    banner.sizeDelta = new Vector2(width, height);
                    banner.anchorMin = new Vector2(0.5f, 1f);
                    banner.anchorMax = new Vector2(0.5f, 1f);
                    banner.anchoredPosition = new Vector2(0, -banner.sizeDelta.y / 2);
                    break;
                case AdType.BigBanner:
                    width = AdSize.MediumRectangle.Width * Screen.dpi / 160 + 10;
                    height = 10 + AdSize.MediumRectangle.Height * Screen.dpi / 160 +10;
                    banner.sizeDelta = new Vector2(width, height);
                    banner.anchoredPosition = new Vector2( 1  * width / 2, height / 2);
                    banner.anchorMin = new Vector2(0f, 0f);
                    banner.anchorMax = new Vector2(0f, 0f);
                    break;
            }
            placeHolder.gameObject.AddComponent<Image>().color = Color.black;
            placeHolder.gameObject.SetActive(false);
        }

        int CalculateBannerHeight()
        {
            if (Screen.height <= 400 * Screen.dpi / 160)
                return (int)(32 * Screen.dpi / 160);
            else if (Screen.height <= 720 * Screen.dpi / 160)
                return (int)(50 * Screen.dpi / 160);
            else
                return (int)(90 * Screen.dpi / 160);
        }
    }
}
#endif