using SH.Ads.Base;
using System;
using System.Collections;
using UnityEngine;

namespace SH.Ads.Piplines
{
    [CreateAssetMenu(fileName = nameof(Waterfall), menuName = "SH/Pipline/Create New/Waterfall", order = 1)]
    public class Waterfall : IPipeline
    {
        public override string Name => "Waterfall Pipline";
        public override string Description =>
            "The Waterfall Pipeline optimizes ad delivery by prioritizing advertisers in sequence. " +
            "\n\nFor example, you can configure it to display ads from AdMob, " +
            "and if unavailable, it will fall back to Unity or any other advertiser you added" +
            "\n\nThis Waterfall Pipeline allows efficient utilization of multiple ad platforms, " +
            "ensuring ads are served from the first available advertiser for each ad type.";

        public override IEnumerator Intialize()
        {
            foreach(var t in Advertisers)
            {
                UnityEngine.Debug.Log("Ad status : Intializing advertiser : "+ t.advertiser);
                yield return t.Initialize();
                UnityEngine.Debug.Log($"Ad status : Advertiser '{t.advertiser}' Intialized");
            }
        }
        public override void ShowAd(AdType adType)
        {
            foreach (var t in Advertisers)
            {
                if (t.ShowAd(adType))
                    return;
            }

            if (adType == AdType.Rewarded || adType == AdType.RewardedInterstial)
                BaseAdHandler.AdNotAvailble();
        }

        public static implicit operator Waterfall(CustomWaterfall v)
        {
            throw new NotImplementedException();
        }
    }
}