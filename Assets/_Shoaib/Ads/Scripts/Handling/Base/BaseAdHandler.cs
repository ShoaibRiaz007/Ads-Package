using SH.API;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SH.Ads.Base
{
    public abstract class BaseAdHandler
    {
        static Transform Canvas = null;
        static GameObject VideoRewardPlaceHolder;
        static Text VideoRewardPlacholderText;
        /// <summary>
        /// Ads Manager Canvas transform
        /// </summary>
        protected Transform CanvasTransform
        {
            get
            {
                if (Canvas == null)
                {
                    Canvas = new GameObject("Ads overlay Canvas", typeof(Canvas), typeof(GraphicRaycaster), typeof(CanvasScaler)).transform;
                    CanvasScaler tem = Canvas.gameObject.GetComponent<CanvasScaler>();
                    tem.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                    tem.referenceResolution = new Vector2(Screen.width, Screen.height);
                    Canvas tem0 = Canvas.gameObject.GetComponent<Canvas>();
                    tem0.renderMode = RenderMode.ScreenSpaceOverlay;
                    tem0.sortingOrder = 32766;
                    var Container = new GameObject("Container").AddComponent<RectTransform>();
                    Container.transform.parent = CanvasTransform;
                    Container.UpdateRectForSafeArea();
                    Container.sizeDelta = new Vector2(0, 0);
                    Container.anchoredPosition = new Vector2(0f, 0f);
                    MonoBehaviour.DontDestroyOnLoad(Canvas);
                }
                return Canvas;
            }
        }

        protected DateTime LastAdShownTime = DateTime.MinValue;
        protected bool CanAdBeShown
        {
            get
            {
                Debug.Log("Ads status : Last Ad call Time : " + (DateTime.Now - LastAdShownTime).TotalSeconds);
                return (DateTime.Now - LastAdShownTime).TotalSeconds > adReshowTime;
            }
        }

        protected static bool TestMode => AdSettings.TestMode;
        /// <summary>
        /// IDs of the ad
        /// </summary>
        protected List<string> IDs;
        /// <summary>
        /// Current Ad ID
        /// </summary>
        protected int count = 0;
        /// <summary>
        /// Time after which a ad can be shown again
        /// </summary>
        protected float adReshowTime = 0;
        /// <summary>
        /// Ad is intialized
        /// </summary>
        protected bool IsIntialized = false;
        /// <summary>
        /// Is ad being ad load
        /// </summary>
        protected bool adLoading = false;
        /// <summary>
        /// Will add load after closing
        /// </summary>
        protected bool loadAfterClose = false;
        /// <summary>
        /// Is ad is showing
        /// </summary>
        static protected internal bool LocalAdShown = false;
        /// <summary>
        /// Check if ad is currently loaded
        /// </summary>
        abstract protected internal bool IsAdAvailable { get; }
        /// <summary>
        /// Check if ad is showing
        /// </summary>
        abstract protected internal bool IsAdShowing { get; protected set; }
        /// <summary>
        /// Intialize the ad
        /// </summary>
        virtual internal void Initialize(AD ad)
        {
            IDs = ad.ADIds;
            loadAfterClose = ad.LoadAfterClose;
            IsIntialized = true;
        }
        /// <summary>
        /// Load ad from the server
        /// </summary>
        abstract protected void Load();
        /// <summary>
        /// Show Ad which is loaded. If not loaded it will load the ad
        /// </summary>
        abstract internal void Show();
        /// <summary>
        /// Hide the ad from the view
        /// </summary>
        abstract internal void Hide();
        /// <summary>
        /// Destroy the ad
        /// </summary>
        abstract internal void Remove();

        /// <summary>
        /// Add UI Place holders
        /// </summary>
        static void AddVideoPlaceHolder()
        {
            if (VideoRewardPlaceHolder != null)
                return;
            
            if(AdSettings.UIRewardedAdTimer == null)
            {
                VideoRewardPlaceHolder = new GameObject("Video ad Placeholder", typeof(RectTransform));
                VideoRewardPlaceHolder.transform.SetParent(Canvas);

                VideoRewardPlaceHolder.AddComponent<Image>().color = new Color(0, 0, 0, 0.5f);
                var container = VideoRewardPlaceHolder.GetComponent<RectTransform>();
                container.anchorMin = Vector2.zero;
                container.anchorMax = Vector2.one;
                container.sizeDelta = new Vector2(0, 0);
                container.anchoredPosition = new Vector2(0f, 0f);

                var tem = new GameObject("Placeholder Text", typeof(RectTransform), typeof(Text));
                tem.transform.SetParent(VideoRewardPlaceHolder.transform);
                container = tem.GetComponent<RectTransform>();
                container.anchorMin = Vector2.zero;
                container.anchorMax = Vector2.one;
                container.sizeDelta = new Vector2(0, 0);
                container.anchoredPosition = new Vector2(0f, 0f);

                VideoRewardPlacholderText = container.GetComponent<Text>();

                VideoRewardPlacholderText.font = Font.CreateDynamicFontFromOSFont(Font.GetOSInstalledFontNames()[0], 50);
                VideoRewardPlacholderText.fontStyle = FontStyle.Bold;
                VideoRewardPlacholderText.fontSize = 50;
                VideoRewardPlacholderText.alignment = TextAnchor.MiddleCenter;
            }
            else
            {
                VideoRewardPlaceHolder = MonoBehaviour.Instantiate(AdSettings.UIRewardedAdTimer, Canvas);
                VideoRewardPlacholderText = VideoRewardPlaceHolder.GetComponentInChildren<Text>();
            }
        }
        /// <summary>
        /// Shows Placeholder screen for 3-2-1 text before advertisment
        /// </summary>
        /// <param name="OnComplete">Called After place holder time completes</param>
        /// <returns></returns>
        protected  static IEnumerator ShowRewardedPlaceholder(Action OnComplete,bool noAdAvailable=false)
        {
            WaitForSecondsRealtime wait = new WaitForSecondsRealtime(1);
            if(VideoRewardPlaceHolder==null)
                AddVideoPlaceHolder();
            yield return null;
            if (noAdAvailable)
            {
                VideoRewardPlaceHolder.gameObject.SetActive(true);
                VideoRewardPlacholderText.text = $"No Ad is available Now. Try again in few minutes";
                yield return wait;
                VideoRewardPlaceHolder.gameObject.SetActive(false);
            }
            else
            {
                if (VideoRewardPlacholderText == null)
                {
                    Debug.LogError("Ad Status : In provided prefab you have not included 'Text' component in Rewarded Video Prefab childrens");
                    yield break;
                }
                Time.timeScale = 0;
                VideoRewardPlaceHolder.gameObject.SetActive(true);
                for (int i = 3; i > 0; i--)
                {
                    VideoRewardPlacholderText.text = $"Showing Rewarded Video Ad in {i} sec";
                    yield return wait;
                }
                VideoRewardPlaceHolder.gameObject.SetActive(false);
                OnComplete?.Invoke();
                Time.timeScale = 1;
            }
           
        }

        internal static void AdNotAvailble()
        {
            AdsManager.BGRunnerInstance.StartCoroutine(ShowRewardedPlaceholder(null, true));
        }
    }
}