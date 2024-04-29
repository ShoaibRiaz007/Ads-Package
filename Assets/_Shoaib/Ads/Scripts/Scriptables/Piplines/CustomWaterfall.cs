using SH.Ads.Base;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SH.Ads.Piplines
{ 
    public class CustomWaterfall : IPipeline
    {
        [System.Serializable]
        public class CustomAdvertiser
        {
            [SerializeField]public SupportedAdvertisers m_Advertiser;
            [SerializeField]public AdType m_AdType;
        }

        [HideInInspector] public List<CustomAdvertiser> m_AdRules =new List<CustomAdvertiser>();


#if UNITY_EDITOR
        public override string Name => "Custom Waterfall Pipline";
        public override string Description =>
            "The Custom Waterfall Pipeline allows you to specify which type of ad each advertiser will display. " +
            "\n\nFor instance, you can configure it to show AdMob's banner and rectangular banner ads, while using Unity or IronSource for interstitial ads. " +
            "\nTo implement this, follow these steps: " +
            "\n1. Create a custom handler for AdMob's banner and rectangular banner ads and set up two interstitial slots for Unity and IronSource. " +
            "\n2. When a banner ad is requested, it will exclusively be served by AdMob. " +
            "\n\nHowever, when an interstitial ad is requested, the pipeline will attempt Unity first, and if Unity's interstitial is not available, it will fall back to IronSource. " +
            "\nThis approach allows you to utilize multiple advertising platforms without concerns about ad limitations from a single advertiser, even when making multiple requests for each ad type.";

        const string Location = "Assets/_Shoaib/Ads/Data/CustomWaterfall.asset";

        public static CustomWaterfall Load()
        {
            var customWaterfallPipline = UnityEditor.AssetDatabase.LoadAssetAtPath<CustomWaterfall>(Location);
            if (customWaterfallPipline == null)
            {
                customWaterfallPipline = CreateInstance<CustomWaterfall>();
                UnityEditor.AssetDatabase.CreateAsset(customWaterfallPipline, Location);
                UnityEditor.AssetDatabase.SaveAssets();
            }
            return customWaterfallPipline;
        } 
#endif
        public override void ShowAd(AdType adType)
        {
            if (m_AdRules.Count == 0)
            {
                base.ShowAd(adType);
                return;
            }


            int shownAdvertiserIndex = ShowRuleAd(adType);

            if (shownAdvertiserIndex != -1)
            {
                Advertisers.Where(a => a.advertiser != Advertisers[shownAdvertiserIndex].advertiser)
                           .ToList()
                           .ForEach(a => a.RemoveAd(adType));
            }
        }

        int ShowRuleAd(AdType adType)
        {
            var rule = m_AdRules.FirstOrDefault(r => r.m_AdType == adType);
            var advertiser = Advertisers.FirstOrDefault(a => rule != null && a.advertiser == rule.m_Advertiser && a.ShowAd(adType));

            if (advertiser == null && (adType == AdType.Rewarded || adType == AdType.RewardedInterstial))
            {
                BaseAdHandler.AdNotAvailble();
            }

            return advertiser?.order ?? -1;
        }


    }
}