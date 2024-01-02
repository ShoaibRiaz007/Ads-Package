#if Admob
using GoogleMobileAds.Api;
using GoogleMobileAds.Ump.Api;
using SH.Ads.Base;
using System;
using UnityEditor.PackageManager;
using UnityEngine;

namespace SH.Ads.Admob
{
    public class OpenAd : BaseAdHandler
    {
        const string _Test_ID = "ca-app-pub-3940256099942544/1033173712";


        AppOpenAd openAd;
        bool firstTIme = true;
        GameObject placeHolder;
        protected internal override bool IsAdAvailable => IsIntialized  && openAd != null && openAd.CanShowAd();
        protected internal override bool IsAdShowing { get; protected set; }
        internal override void Intialize(AD ad)
        {
            IDs = ad.ADIds;
            IsIntialized = true;
            firstTIme = true;
            adReshowTime = ad.ADReshowTime;
            CreatePlaceHolder();
            Debug.Log(this + " is intialized with " + IDs.Count + " ad Ids");
            loadAfterClose = ad.LoadAfterClose;
            if (ad.LoadAtStart)
                Load();
        }
        protected override void Load()
        {
            if (adLoading || !IsIntialized || !ConsentInformation.CanRequestAds())
                return;

            if ( IDs.Count > 0)
            {
                AppOpenAd.Load(TestMode ? _Test_ID : IDs[count], AdRequestBuild, Callback);
                adLoading = true;
            }
        }

        internal override void Hide()
        {
            if (openAd != null)
                openAd.Destroy();
            IsAdShowing = false;
        }
        internal override void Remove()
        {
            if (openAd != null)
                openAd.Destroy();
            IsAdShowing = false;
        }
        internal override void Show()
        {
            if ( IDs.Count == 0)
                return;

            if (!CanAdBeShown)
                return;
            if (LocalAdShown)//Don't show or load open ad if Local ad is shown
            {
                LocalAdShown = false;
                return;
            }

            if (IsAdAvailable)
            {
                IsAdShowing = true;
                placeHolder.gameObject.SetActive(true);
                openAd.Show();
                AdsManager.LogAnalyticEvent(this.ToString(), "On_Show", count.ToString());
                LastAdShownTime =DateTime.Now;
            }
            else
            {
               Load();
            }
        }
        private void Callback(AppOpenAd ad, LoadAdError error)
        {
            if (error != null)
            {
                openAd = null;
                placeHolder.gameObject.SetActive(false);
                AdsManager.LogAnalyticEvent(this.ToString(), "On_Fail", error.GetCause().GetMessage());
                Debug.Log($"Ad log : {this} Failed :  {count} cause : {error.GetCause()}");
                if (count + 1 <  IDs.Count)
                {
                    count++;
                    Load();
                    return;
                }
                count = 0;
                IsAdShowing = false;
                return;
            }
            else
            {
                openAd = ad;
               
                openAd.OnAdFullScreenContentClosed += ()=> 
                {
                    adLoading = false;
                    IsAdShowing = false;
                    placeHolder.gameObject.SetActive(false);
                    if (loadAfterClose)
                        Load();
                };
                openAd.OnAdFullScreenContentOpened += () => { placeHolder.gameObject.SetActive(true); };
                openAd.OnAdImpressionRecorded += () =>
                {
                    AdsManager.LogAnalyticEvent(this.ToString(), "On_Impression", count.ToString());
                    Debug.Log("Ad log : Open impression Recorded :  " + count);
                };
                openAd.OnAdPaid += (v) =>
                {
                    AdsManager.LogAnalyticEvent(this.ToString(), "On_Paid", v.Value.ToString());
                    Debug.Log("Ad log : Open ad paid :  " + v.Value);
                };

                if (firstTIme)
                {
                    firstTIme = false;
                    Show();
                }
                AdsManager.LogAnalyticEvent(this.ToString(), "On_Load", count.ToString());
            }
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
        void CreatePlaceHolder()
        {
            placeHolder = new GameObject("OpenAd Placeholder", typeof(RectTransform));
            placeHolder.transform.parent = CanvasTransform;
            var rect = placeHolder.GetComponent<RectTransform>();
           
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
            rect.anchoredPosition = Vector2.zero;

            placeHolder.gameObject.AddComponent<UnityEngine.UI.Image>().color = Color.black;
            placeHolder.gameObject.SetActive(false);


#if UNITY_EDITOR
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += (v,a)=> placeHolder.gameObject.SetActive(false);
#endif
        }
    }
}
#endif