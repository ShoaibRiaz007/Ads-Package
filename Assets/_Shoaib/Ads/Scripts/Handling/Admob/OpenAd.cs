#if Admob
using GoogleMobileAds.Api;
using GoogleMobileAds.Ump.Api;
using SH.Ads.Base;
using System;
using UnityEngine;

namespace SH.Ads.Admob
{
    public class OpenAd : BaseAdHandler
    {
        const string TestAdUnitId = "ca-app-pub-3940256099942544/1033173712";

        private AppOpenAd openAd;
        private bool firstTime = true;
        private GameObject placeHolder;

        protected internal override bool IsAdAvailable => IsIntialized && openAd != null && openAd.CanShowAd();
        protected internal override bool IsAdShowing { get; protected set; }

        internal override void Initialize(AD ad)
        {
            IDs = ad.ADIds;
            IsIntialized = true;
            firstTime = true;
            adReshowTime = ad.ADReshowTime;
            CreatePlaceHolder();
            loadAfterClose = ad.LoadAfterClose;
            Debug.Log($"{this} is initialized with {IDs.Count} ad Ids");
            if (ad.LoadAtStart)
                Load();
        }

        protected override void Load()
        {
            if (adLoading || !IsIntialized || !ConsentInformation.CanRequestAds())
                return;

            if (IDs.Count > 0)
            {
                string adUnitId = TestMode ? TestAdUnitId : IDs[count];
                AdRequest request = BuildAdRequest();

                AppOpenAd.Load(adUnitId, request, (loadedAd, error) =>
                {
                    if (error != null)
                    {
                        Debug.Log($"Ad log: {this} Failed to load: {error?.GetCode()}");
                        AdsManager.LogAnalyticEvent(this.ToString(), "On_Fail", error?.GetCode().ToString());
                        if (++count < IDs.Count)
                            Load();
                        else
                            count = 0;
                    }
                    else
                    {
                        openAd = loadedAd;
                        openAd.OnAdFullScreenContentClosed += OnAdClosed;
                        openAd.OnAdFullScreenContentOpened += OnAdOpened;
                        openAd.OnAdImpressionRecorded += OnAdImpressionRecorded;
                        openAd.OnAdPaid += OnAdPaid;

                        Debug.Log($"Ad log: {this} Loaded");
                        AdsManager.LogAnalyticEvent(this.ToString(), "On_Load", count.ToString());

                        if (firstTime)
                        {
                            firstTime = false;
                            Show();
                        }
                    }
                });

                adLoading = true;
            }
        }

        internal override void Hide()
        {
            if (openAd != null)
                openAd.Destroy();

            IsAdShowing = false;
            placeHolder?.SetActive(false);
        }

        internal override void Remove()
        {
            if (openAd != null)
                openAd.Destroy();

            IsAdShowing = false;
            placeHolder?.SetActive(false);
        }

        internal override void Show()
        {
            if (IDs.Count == 0 || !CanAdBeShown)
                return;

            if (LocalAdShown)
            {
                LocalAdShown = false;
                return;
            }

            if (IsAdAvailable)
            {
                openAd.Show();
                LastAdShownTime = DateTime.Now;
            }
            else
            {
                Load();
            }
        }

        private void OnAdClosed()
        {
            IsAdShowing = false;
            placeHolder?.SetActive(false);
            if (loadAfterClose)
                Load();
        }

        private void OnAdOpened()
        {
            IsAdShowing = true; 
            placeHolder?.SetActive(true);
            AdsManager.LogAnalyticEvent(this.ToString(), "On_Show", count.ToString());
        }

        private void OnAdImpressionRecorded()
        {
            AdsManager.LogAnalyticEvent(this.ToString(), "On_Impression", count.ToString());
            Debug.Log($"Ad log: Open impression recorded");
        }

        private void OnAdPaid(AdValue args)
        {
            AdsManager.LogAnalyticEvent(this.ToString(), "On_Paid", args.Value.ToString());
            Debug.Log($"Ad log: Open ad paid: {args.Value}");
        }

        private AdRequest BuildAdRequest()
        {
            var tem = new AdRequest();
            tem.Extras.Add("npa", AdSettings.GDPRConcent ? "0" : "1");
            tem.Extras.Add("rdp", AdSettings.CCPAConsent ? "0" : "1");
            tem.Extras.Add("is_designed_for_families", AdSettings.IsForChildren ? "true" : "false");
            return tem;
        }

        private void CreatePlaceHolder()
        {
            placeHolder = new GameObject("OpenAd Placeholder", typeof(RectTransform));
            placeHolder.transform.SetParent(CanvasTransform);
            var rect = placeHolder.GetComponent<RectTransform>();

            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
            rect.anchoredPosition = Vector2.zero;

            placeHolder.gameObject.AddComponent<UnityEngine.UI.Image>().color = Color.black;
            placeHolder.gameObject.SetActive(false);

#if UNITY_EDITOR
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += (v, a) => placeHolder.gameObject.SetActive(false);
#endif
        }
    }
}
#endif
