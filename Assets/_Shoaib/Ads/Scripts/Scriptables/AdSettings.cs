using SH.Ads.API.UI;
using SH.Ads.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SH.Ads
{
    [CreateAssetMenu(fileName = "AdSetting", menuName = "SH/Ads/Ads Setting/Create New", order = 1)]
    public class AdSettings : ScriptableObject
    {
        static AdSettings _instance = null;

        static AdSettings Instance 
        { 
            get 
            { 
                if (_instance == null)
                    _instance = Resources.Load<AdSettings>(nameof(AdSettings));
                return _instance;
            }
        }

        public enum AppAgeRatting { G,PG,T,MA}


        [SerializeField, Header("App Setting")] bool m_IsForChildren = false;
        [SerializeField,Tooltip("G -> All age group \n PG -> parent guiduence needed \n T -> 13+ years \n MA -> 16+ || 18+")] AppAgeRatting m_AgeRating = AppAgeRatting.G;


        [SerializeField, Header("Ads Setting")] float m_RewardAmountForRewardedAds = 500;

        [SerializeField,Tooltip("Show Test Ads")] bool m_TestMode ;
        [SerializeField]List<string> m_TestDeviceIDs = new List<string>();


        [SerializeField, Header("If null then default assets would be used"), Header("UI Prefabs")]
        RewardPanel m_UIRewardedAdPanel;
        [SerializeField]GameObject m_UIRewardedAdTimer;

        public List<Adons.AdOn> m_AdOns = new List<Adons.AdOn>();

        [HideInInspector] public IPipeline CurrentPipline;
        internal static IEnumerator InitializeAdsHandler(Action OnComplete=null)
        {
            foreach(var t in Instance.m_AdOns)
            {
                yield return t.Initialize(Instance);
            }
            yield return null;
            
            if(!RemoveAd)
                yield return Instance.CurrentPipline.Initialize();

            OnComplete?.Invoke();
        }
        internal static void LogAnalyticEvent(string eventName, string parameterName, string parameterValue)
        {
#if FirebaseAnalytics
            foreach (var adOn in Instance.m_AdOns)
            {
                if (adOn is Adons.FirebaseAnalytics FA)
                {
                    FA.LogEvent(eventName, parameterName,parameterValue);
                    break;
                }
            }
#endif
        }
        internal static void LogAnalyticEvent(string eventName)
        {
#if FirebaseAnalytics
            foreach (var adOn in Instance.m_AdOns)
            {
                if (adOn is Adons.FirebaseAnalytics FA)
                {
                    FA.LogEvent(eventName);
                    break;
                }
            }
#endif
        }

#if FirebaseAnalytics
        internal static void LogAnalyticEvent(string eventName,params Firebase.Analytics.Parameter[] parameters)
        {
            foreach (var adOn in Instance.m_AdOns)
            {
                if (adOn is Adons.FirebaseAnalytics FA)
                {
                    FA.LogEvent(eventName, parameters);
                    break;
                }
            }
        }
#endif

        internal static void ReviewApp(Action<bool> Success)
        {
#if GoogleReview
            foreach (var adOn in Instance.m_AdOns)
            {
                if (adOn is Adons.GoogleReview GR)
                {
                    GR.RequestRateUs(Success);
                    break;
                }
            }
#endif

        }

        internal static void AdCalling(AdType adType) => Instance.CurrentPipline.ShowAd(adType);
        internal static void RemoveVisibleAd(AdType adType) => Instance.CurrentPipline.RemoveAd(adType);


        private void OnValidate()
        {
            if (_instance == null)
                _instance = Resources.Load<AdSettings>(nameof(AdSettings));
        }
        private void OnEnable()
        {
            if (_instance == null)
                _instance = Resources.Load<AdSettings>(nameof(AdSettings));
        }
#if UNITY_EDITOR
        private const string Location = "Assets/_Shoaib/Ads/Resources";
        public static AdSettings Load()
        {
            var adSettings = Resources.Load<AdSettings>(nameof(AdSettings));
            if (adSettings == null)
            {
                adSettings = CreateInstance<AdSettings>();
                if (!System.IO.Directory.Exists(Location))
                {
                    System.IO.Directory.CreateDirectory(Location);
                }
                string assetPath = $"{Location}/{nameof(AdSettings)}.asset";
                UnityEditor.AssetDatabase.CreateAsset(adSettings, assetPath);
                UnityEditor.AssetDatabase.SaveAssets();
                UnityEditor.Selection.activeObject = adSettings;
            }

            return adSettings;
        }
#endif

        #region Properties

        public static bool RemoveAd
        {
            get => PlayerPrefs.GetInt("AdsRemove", 0) == 1;
            set
            {
                if (value)
                {
                    RemoveVisibleAd(AdType.Banner);
                    RemoveVisibleAd(AdType.BigBanner);
                }
                PlayerPrefs.SetInt("AdsRemove", value ? 1 : 0);
            }
        }

        public static bool GDPRConcent
        {
            get => PlayerPrefs.GetInt("AdsNPAConcent", 0) == 1;
            set => PlayerPrefs.SetInt("AdsNPAConcent", value ? 1 : 0);
        }

        public static bool CCPAConsent
        {
            get => PlayerPrefs.GetInt("AdsCCPAConsent", 0) == 1;
            set => PlayerPrefs.SetInt("AdsCCPAConsent", value ? 1 : 0);
        }

        public static bool IsForChildren => Instance?.m_IsForChildren ?? false;

        public static string AgeGroupRating => Instance?.m_AgeRating.ToString() ?? "";

        public static bool TestMode => Instance?.m_TestMode ?? true;

        public static List<string> TestDevices => Instance?.m_TestDeviceIDs;

        public static float RewardAmount => Instance?.m_RewardAmountForRewardedAds ?? 500;

        public static GameObject UIRewardedAdTimer => Instance?.m_UIRewardedAdTimer;

        public static RewardPanel UIRewardAdPanel => Instance?.m_UIRewardedAdPanel;

        #endregion
    }
}