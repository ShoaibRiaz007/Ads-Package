using SH.Ads.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SH.Ads.Piplines
{ 
    [CreateAssetMenu(fileName = nameof(CustomWaterfall), menuName = "SH/Pipline/Create New/CustomWaterfall", order = 1)]
    public class CustomWaterfall : IPipeline
    {
        [System.Serializable]
        public class CustomAdvertiser
        {
            [SerializeField]public SupportedAdvertisers m_Advertiser;
            [SerializeField]public AdType m_AdType;

            public void UpdateAdType(AdType adType)
            {
                m_AdType = adType;
                Debug.LogError(adType);
            }
            public void UpdateAdvertiser(SupportedAdvertisers advertisers)
            {
                m_Advertiser = advertisers;
            }
        }

        [HideInInspector] public List<CustomAdvertiser> m_AdRules =new List<CustomAdvertiser>();

        public override string Name => "Custom Waterfall Pipline";
        public override string Description => 
            "The Custom Waterfall Pipeline allows you to specify which type of ad each advertiser will display. " +
            "\n\nFor instance, you can configure it to show AdMob's banner and rectangular banner ads, while using Unity or IronSource for interstitial ads. " +
            "\nTo implement this, follow these steps: " +
            "\n1. Create a custom handler for AdMob's banner and rectangular banner ads and set up two interstitial slots for Unity and IronSource. " +
            "\n2. When a banner ad is requested, it will exclusively be served by AdMob. " +
            "\n\nHowever, when an interstitial ad is requested, the pipeline will attempt Unity first, and if Unity's interstitial is not available, it will fall back to IronSource. " +
            "\nThis approach allows you to utilize multiple advertising platforms without concerns about ad limitations from a single advertiser, even when making multiple requests for each ad type.";
             
        public override IEnumerator Intialize()
        {
            foreach(var t in Advertisers)
            {
                Debug.Log("Ad status : Intializing advertiser : "+ t.advertiser);
                yield return t.Initialize();
                Debug.Log($"Ad status : Advertiser '{t.advertiser}' Intialized");
            }
        }
        public override void ShowAd(AdType adType)
        {
            foreach (var t in m_AdRules)
            {
                if(adType == t.m_AdType)
                {
                    foreach(var ad in Advertisers)
                    {
                        if(ad.ShowAd(adType))
                            return;
                    }
                    
                }
            }

            if (adType == AdType.Rewarded || adType == AdType.RewardedInterstial)
                BaseAdHandler.AdNotAvailble();
        }
    }
}