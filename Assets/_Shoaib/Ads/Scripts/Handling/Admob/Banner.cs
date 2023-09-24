#if Admob
using GoogleMobileAds.Api;
using SH.Ads.Base;
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
        internal override void Intialize(AD ad)
        {
            CreatePlaceHolder();
            IDs = ad.adIds;
            adType = ad.adType;
            IsIntialized = true;
            loadAfterClose = ad.loadAfterClose;
            if (ad.loadAtStart)
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
                switch (adType)
                {
                    case AdType.Banner:
                        bannerView = new BannerView(TestMode ? _Test_ID : IDs[count],Screen.orientation == ScreenOrientation.Landscape? AdSize.GetLandscapeAnchoredAdaptiveBannerAdSizeWithWidth(Screen.width) : AdSize.GetPortraitAnchoredAdaptiveBannerAdSizeWithWidth(Screen.width), AdPosition.Top);
                        break;
                    case AdType.BigBanner:
                        bannerView = new BannerView(TestMode ? _Test_ID : IDs[count], AdSize.MediumRectangle, AdPosition.BottomLeft);
                        break;
                }
                bannerView.OnBannerAdLoadFailed += (arg)=> 
                {
                    Debug.Log($"Ad log : {this} Failed :  {count} cause : {arg.GetCause()}");
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
                    placeHolder?.gameObject.SetActive(true);
                    adLoading = false;
                };
                bannerView.OnAdImpressionRecorded += () =>
                {
                    Debug.Log("Ad log : Banner impression Recorded :  " + count);
                };
                bannerView.OnAdPaid += (v) =>
                {
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

            placeHolder?.gameObject.SetActive(false);
        }
        internal override void Remove()
        {
            if (IsAdAvailable)
                bannerView.Destroy();
            placeHolder?.gameObject.SetActive(false);
        }
        internal override void Show()
        {
            if ( IDs.Count == 0)
                return;

            if (IsAdAvailable)
            {
                placeHolder?.gameObject.SetActive(true);
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
                    width = Screen.width;
                    height =CalculateBannerHeight() + 15;
                    banner.sizeDelta = new Vector2(width, height == 0 ? CalculateBannerHeight() : height);
                    banner.anchorMin = new Vector2(0.5f, 1f);
                    banner.anchorMax = new Vector2(0.5f, 1f);
                    banner.anchoredPosition = new Vector2(0, -banner.sizeDelta.y / 2);
                    break;
                case AdType.BigBanner:
                    width = AdSize.MediumRectangle.Width * Screen.dpi / 160 + 10;
                    height = 10 + AdSize.MediumRectangle.Height * Screen.dpi / 160;
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