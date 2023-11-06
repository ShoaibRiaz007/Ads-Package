using SH.Ads.Base;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SH.Ads
{
    [CreateAssetMenu(fileName = "AdSetting", menuName = "SH/Ads Setting/Create New", order = 1)]
    public class AdSettings : ScriptableObject
    {
        static AdSettings _instance = null;

        static AdSettings Instance 
        { 
            get 
            { 
                if (_instance == null)
                    _instance = Resources.Load<AdSettings>("AdSetting");
                return _instance;
            }
        }

        public enum AppAgeRatting { G,PG,T,MA}


        [SerializeField, Header("App Setting")] bool m_IsForChildren = false;
        [SerializeField,Tooltip("G -> All age group \n PG -> parent guiduence needed \n T -> 13+ years \n MA -> 16+ || 18+")] AppAgeRatting m_AgeRating = AppAgeRatting.G;


        [SerializeField, Header("Ads Setting")] float m_RewardAmountForRewardedAds = 500;
        [SerializeField,Tooltip("Show Test Ads")] bool m_TestMode ;
        [SerializeField]List<string> m_TestDeviceIDs = new List<string>();
        [HideInInspector]public List<Advertiser> advertisers = new List<Advertiser>();


        [SerializeField, Header("If null then default assets would be used"), Header("UI Prefabs")]
        GameObject m_RewardedAdsPrefab;

        internal static  List<Advertiser> Advertisers => Instance?.advertisers; 
        private void OnValidate()
        {
            if (_instance == null)
            {
                _instance = Resources.Load<AdSettings>("AdSetting");
            }
        }

        private void OnEnable()
        {
            if (_instance == null)
            {
                _instance = Resources.Load<AdSettings>("AdSetting");
            }
        }

        [field: NonSerialized] public static bool RemoveAd { get => PlayerPrefs.GetInt("AdsRemove", 0) == 1; set => PlayerPrefs.GetInt("AdsRemove", value ? 1 : 0); }
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