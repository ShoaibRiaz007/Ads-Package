using System;
using System.Collections;
using UnityEngine;

namespace SH.Ads
{
    /// <summary>
    /// Ads Manager to control all the ads behavior
    /// </summary>
    public static class AdsManager
    {
        internal static Action<string, float> OnUserEarnedReward;

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
                    foreach (var t in AdSettings.Advertisers)
                    {
                        if (t.ShowAd(AdType.OpenAd))
                            return;
                    }
                }
            }

            public new void StartCoroutine(IEnumerator function)
            {
                base.StartCoroutine(function);
            }
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
            BGRunnerInstance.StartCoroutine(DelayInitializeAdvertisers());
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
            BGRunnerInstance.StartCoroutine(DelayInitializeAdvertisers(OnComplete));
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

        static IEnumerator DelayInitializeAdvertisers(Action OnComplete=null)
        {
            yield return null;

            foreach (var t in AdSettings.Advertisers)
            {
                Debug.Log("Ads Status : Initializing " + t.advertiser);
                yield return t.Initialize();
            }
            OnComplete?.Invoke();
        }
        /// <summary>
        /// Will show banner at top of screen
        /// </summary>
        public static void ShowBanner()
        {
            foreach (var t in AdSettings.Advertisers)
            {
                if (t.ShowAd(AdType.Banner))
                    return;
            }
        }
        /// <summary>
        /// Will Show any Big banner if available at bottom right
        /// Note : Only banner or big banner can show at a time for single Advertiser
        /// </summary>
        public static void ShowBigBanner()
        {
            foreach (var t in AdSettings.Advertisers)
            {
                if (t.ShowAd(AdType.BigBanner))
                    return;
            }
        }
        /// <summary>
        /// Will Show any interstial if available
        /// </summary>
        public static void ShowInterstitial()
        {
            foreach (var t in AdSettings.Advertisers)
            {
                if (t.ShowAd(AdType.Interstial))
                    return;
            }
        }
        /// <summary>
        /// Show Rewarded video ad
        /// </summary>
        /// <param name="OnAdReward">Will call when user completes rewarded ad</param>
        public static void ShowRewarded(Action<string, float> OnAdReward)
        {
            OnUserEarnedReward = OnAdReward;
            foreach (var t in AdSettings.Advertisers)
            {
                if (t.ShowAd(AdType.Rewarded))
                    return;
            }
            Base.BaseAdHandler.AdNotAvailble();
        }
        /// <summary>
        /// Show Rewarded Interstial Ad
        /// </summary>
        /// <param name="OnAdReward">Will call when user completes rewarded ad</param>
        public static void ShowRewardedInterstitial(Action<string, float> OnAdReward)
        {
            OnUserEarnedReward = OnAdReward;
            foreach (var t in AdSettings.Advertisers)
            {
                if (t.ShowAd(AdType.RewardedInterstial))
                    return;
            }
            Base.BaseAdHandler.AdNotAvailble();
        }
        /// <summary>
        /// Will try to show Reward or Rewarded Interstial
        /// </summary>
        /// <param name="OnAdReward">Will call when user completes rewarded ad</param>
        public static void ShowAnyRewarded(Action<string, float> OnAdReward)
        {
            OnUserEarnedReward = OnAdReward;
            foreach (var t in AdSettings.Advertisers)
            {
                if (t.ShowAd(AdType.Rewarded))
                    return;
            }
            foreach (var t in AdSettings.Advertisers)
            {
                if (t.ShowAd(AdType.RewardedInterstial))
                    return;
            }

            Base.BaseAdHandler.AdNotAvailble();
        }
    }
}
