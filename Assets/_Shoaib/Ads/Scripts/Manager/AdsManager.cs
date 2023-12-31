using SH.Ads.Base;
using System;
using System.Collections;
using UnityEngine;
#if IronSource
using IrosourceAgent = IronSource;
#endif
namespace SH.Ads
{
    /// <summary>
    /// Ads Manager to control all the ads behavior
    /// </summary>
    public static class AdsManager
    {
        internal static Action<string, float> OnUserEarnedReward;

        static AdsManager()
        {
            OnUserEarnedReward = null;
        }
        
        static BGRunner _instance = null;
        internal static BGRunner BGRunnerInstance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameObject("BG Runner").AddComponent<BGRunner>();
                }
                return _instance;
            }
        }

        internal class BGRunner : MonoBehaviour
        {
            private void Start()
            {
                DontDestroyOnLoad(gameObject);
            }

            private void OnApplicationFocus(bool onFocus)
            {
                if (onFocus)
                {
                   AdSettings.AdCalling(AdType.OpenAd);
                }


            }

            private void OnApplicationPause(bool pause)
            {
#if IronSource
                IrosourceAgent.Agent.onApplicationPause(pause);
#endif
            }

            public new void StartCoroutine(IEnumerator function)
            {
                base.StartCoroutine(function);
            }
        }
        /// <summary>
        /// Log an Event on firebase analystics
        /// </summary>
        /// <param name="EventName">Event Name like [Level_Complete_]</param>
        /// <param name="ParameterName">Parameter name like [Level_]</param>
        /// <param name="ParameterValue">Parameter Value Like [1] or ["Completed"]</param>
        public static void LogAnalyticEvent(string EventName,string ParameterName, string ParameterValue)
        {
#if FirebaseAnalytics
            Firebase.Analytics.FirebaseAnalytics.LogEvent(EventName, ParameterName, ParameterValue);
#else
            Debug.Log("Ad log : Firebase analytics is not enbaled");
#endif
        }

        /// <summary>
        /// Intialize All Advertisers
        /// </summary>
        /// <param name="NPAConcent"></param>
        /// <param name="CCPAConent"></param>
        public static void Initialize(bool NPAConcent, bool CCPAConent)
        {
            AdSettings.GDPRConcent = NPAConcent;
            AdSettings.CCPAConsent = CCPAConent;
            Debug.Log("Bg thread instance created with object name" + BGRunnerInstance.name);
            BGRunnerInstance.StartCoroutine(AdSettings.IntializeAdsHandler());
        }
        /// <summary>
        /// Intialize All Advertisers
        /// </summary>
        /// <param name="NPAConcent"></param>
        /// <param name="CCPAConent"></param>
        /// <param name="OnComplete">Will call after completing all Advertiser Intializatn</param>
        public static void Initialize(bool NPAConcent, bool CCPAConent, Action OnComplete)
        {
            AdSettings.GDPRConcent = NPAConcent;
            AdSettings.CCPAConsent = CCPAConent;
            Debug.Log("Bg thread instance created with object name" + BGRunnerInstance.name);
            BGRunnerInstance.StartCoroutine(AdSettings.IntializeAdsHandler(OnComplete));
        }
        /// <summary>
        /// Update user concent
        /// </summary>
        /// <param name="NPAConcent"></param>
        /// <param name="CCPAConent"></param>
        public static void UpdateConsent(bool NPAConcent, bool CCPAConent)
        {
            AdSettings.GDPRConcent = NPAConcent;
            AdSettings.CCPAConsent = CCPAConent;
        }
        /// <summary>
        /// Will show banner at top of screen
        /// </summary>
        public static void ShowBanner()
        {
            AdSettings.AdCalling(AdType.Banner);
        }
        /// <summary>
        /// Will Show any Big banner if available at bottom right
        /// Note : Only banner or big banner can show at a time for single Advertiser
        /// </summary>
        public static void ShowBigBanner()
        {
            AdSettings.AdCalling(AdType.BigBanner);
        }
        /// <summary>
        /// Will Show any interstial if available
        /// </summary>
        public static void ShowInterstitial()
        {
            AdSettings.AdCalling(AdType.Interstial);
        }
        /// <summary>
        /// Show Rewarded video ad
        /// </summary>
        /// <param name="OnAdReward">Will call when user completes rewarded ad</param>
        public static void ShowRewarded(Action<string, float> OnAdReward)
        {
            OnUserEarnedReward = OnAdReward;
            AdSettings.AdCalling(AdType.Rewarded);
        }
        /// <summary>
        /// Show Rewarded Interstial Ad
        /// </summary>
        /// <param name="OnAdReward">Will call when user completes rewarded ad</param>
        public static void ShowRewardedInterstitial(Action<string, float> OnAdReward)
        {
            OnUserEarnedReward = OnAdReward;
            AdSettings.AdCalling(AdType.RewardedInterstial);
        }
        /// <summary>
        /// Will try to show Reward or Rewarded Interstial
        /// </summary>
        /// <param name="OnAdReward">Will call when user completes rewarded ad</param>
        public static void ShowAnyRewarded(Action<string, float> OnAdReward)
        {
            OnUserEarnedReward = OnAdReward;
            AdSettings.AdCalling(AdType.Rewarded);

            Debug.LogError("ToDO");
        }
    }
}
