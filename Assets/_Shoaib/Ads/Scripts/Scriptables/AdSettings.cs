using SH.Ads.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
        GameObject m_RewardedAdsPrefab;

        public List<Adons.AdOn> m_AdOns = new List<Adons.AdOn>();

        [HideInInspector] public IPipeline CurrentPipline;
        internal static IEnumerator IntializeAdsHandler(Action OnComplete=null)
        {
            foreach(var t in Instance.m_AdOns)
            {
                yield return t.Intialize(Instance);
            }
            yield return null;
            
            if(!RemoveAd)
                yield return Instance.CurrentPipline.Intialize();

            OnComplete?.Invoke();
        }
        internal static void LogAnalyticEvent(string eventName, string parameterName, string parameterValue)
        {
#if FirebaseAnalytics
            foreach(var t in Instance.m_AdOns)
            {
                if(t.GetType() == typeof(Adons.FirebaseAnalytics))
                {
                    Adons.FirebaseAnalytics FA = t as Adons.FirebaseAnalytics ;
                    FA.LogEvent(eventName,parameterName,parameterValue);
                }
            }
#endif
        }
        internal static void LogAnalyticEvent(string eventName)
        {
#if FirebaseAnalytics
            foreach (var t in Instance.m_AdOns)
            {
                if (t.GetType() == typeof(Adons.FirebaseAnalytics))
                {
                    Adons.FirebaseAnalytics FA = t as Adons.FirebaseAnalytics;
                    FA.LogEvent(eventName);
                }
            }
#endif
        }

#if FirebaseAnalytics
        internal static void LogAnalyticEvent(string eventName,params Firebase.Analytics.Parameter[] parameters)
        {
            foreach (var t in Instance.m_AdOns)
            {
                if (t.GetType() == typeof(Adons.FirebaseAnalytics))
                {
                    Adons.FirebaseAnalytics FA = t as Adons.FirebaseAnalytics;
                    FA.LogEvent(eventName, parameters);
                }
            }
        }
#endif

        internal static void ReviewApp(Action<bool> Success)
        {
#if GoogleReview
            foreach (var t in Instance.m_AdOns)
            {
                if (t.GetType() == typeof(Adons.GoogleReview))
                {
                    Adons.GoogleReview FA = t as Adons.GoogleReview;
                    FA.RequestRateUs(Success);
                }
            }
#endif
        }

        internal static void AdCalling(AdType adType) => Instance.CurrentPipline.ShowAd(adType);
        internal static void RemoveVisibleAd(AdType adType) => Instance.CurrentPipline.ShowAd(adType);


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
        const string Location = "Assets/_Shoaib/Ads/Resources";
        public static AdSettings Load()
        {
            var AdSetting = Resources.Load<AdSettings>(nameof(AdSettings));
            if (AdSetting == null)
            {
                AdSetting = CreateInstance<AdSettings>();
                if (!Directory.Exists(Location))
                {
                    Directory.CreateDirectory(Location);
                }
                string assetPath = $"{Location}/{nameof(AdSettings)}.asset";
                UnityEditor.AssetDatabase.CreateAsset(AdSetting, assetPath);
                UnityEditor.AssetDatabase.SaveAssets();
                UnityEditor.Selection.activeObject = AdSetting;
            }

            return AdSetting;
        }
#endif

        [field: NonSerialized] public static bool RemoveAd { get => PlayerPrefs.GetInt("AdsRemove", 0) == 1; set { if (value) { RemoveVisibleAd(AdType.Banner); RemoveVisibleAd(AdType.BigBanner); } PlayerPrefs.GetInt("AdsRemove", value ? 1 : 0); } }
        [field: NonSerialized] public static bool GDPRConcent { get => PlayerPrefs.GetInt("AdsNPAConcent", 0) == 1; set => PlayerPrefs.GetInt("AdsNPAConcent", value ?1 : 0);}
        [field: NonSerialized] public static bool CCPAConsent { get => PlayerPrefs.GetInt("AdsCCPAConsent", 0) == 1; set => PlayerPrefs.GetInt("AdsCCPAConsent", value ? 1 : 0); }
                
        [field:NonSerialized]internal static bool IsForChildren => _instance? _instance.m_IsForChildren:false;
        [field: NonSerialized] public static string AgeGroupRating=> _instance?.m_AgeRating.ToString();
        [field: NonSerialized] public static bool TestMode => _instance? _instance.m_TestMode:true;

        [field: NonSerialized] public static List<string> TestDevices => _instance?.m_TestDeviceIDs;

        [field: NonSerialized] public static float RewardAmount = _instance? _instance.m_RewardAmountForRewardedAds:500;

        [field: NonSerialized] public static GameObject RewardedAdsPrefab => _instance?.m_RewardedAdsPrefab;
    }
}