#if Admob
using GoogleMobileAds.Api;
using SH.Ads.Base;
using System;
using UnityEngine;

namespace SH.Ads.Admob
{
    public class OpenAd : BaseAdHandler
    {
        const string _Test_ID = "ca-app-pub-3940256099942544/1033173712";


        AppOpenAd openAd;
        float delayTime = 60;
        bool firstTIme = true;
        GameObject placeHolder;
        protected internal override bool IsAdAvailable => IsIntialized  && openAd != null && openAd.CanShowAd();
        internal override void Intialize(AD ad)
        {
            IDs = ad.adIds;
            IsIntialized = true;
            firstTIme = true;
            CreatePlaceHolder();
            Debug.Log(this + " is intialized with " + IDs.Count + " ad Ids");
            loadAfterClose = ad.loadAfterClose;
            if (ad.loadAtStart)
                Load();
        }
        protected override void Load()
        {
            if (adLoading || !IsIntialized)
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
        }
        internal override void Remove()
        {
            if (openAd != null)
                openAd.Destroy();
        }
        internal override void Show()
        {
            if ( IDs.Count == 0)
                return;

            if (!OpenAdTime)
                return;

            if (IsAdAvailable)
            {
                LocalAdShown = false;
                placeHolder.gameObject.SetActive(true);
                openAd.Show();
                lastAdwasLoaded=DateTime.Now;
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
                Debug.Log($"Ad log : {this} Failed :  {count} cause : {error.GetCause()}");
                if (count + 1 <  IDs.Count)
                {
                    count++;
                    Load();
                    return;
                }
                count = 0;
                return;
            }
            else
            {
                openAd = ad;
               
                openAd.OnAdFullScreenContentClosed += ()=> 
                {
                    adLoading = false;
                    placeHolder.gameObject.SetActive(false);
                    if (loadAfterClose)
                        Load();
                };
                openAd.OnAdFullScreenContentOpened += () => { placeHolder.gameObject.SetActive(true); };
                openAd.OnAdImpressionRecorded += () =>
                {
                     Debug.Log("Ad log : Open impression Recorded :  " + count);
                };
                openAd.OnAdPaid += (v) =>
                {
                    Debug.Log("Ad log : Open ad paid :  " + v.Value);
                };

                if (firstTIme)
                {
                    firstTIme = false;
                    Show();
                }
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
        DateTime lastAdwasLoaded = DateTime.MinValue;
        bool OpenAdTime
        {
            get
            {
                Debug.Log("Ads status : Last Open Ad call " + (DateTime.Now - lastAdwasLoaded).TotalSeconds);
                return (DateTime.Now - lastAdwasLoaded).TotalSeconds > delayTime;
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
        }
    }
}
#endif