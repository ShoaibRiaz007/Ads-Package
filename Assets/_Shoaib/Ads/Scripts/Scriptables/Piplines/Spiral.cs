using SH.Ads.Base;

namespace SH.Ads.Piplines
{
    public class Spiral : IPipeline
    {
#if UNITY_EDITOR
        public override string Name => "Spiral Pipline";
        public override string Description =>
            "New layout";
#endif


        public override void ShowAd(AdType adType)
        {
            foreach (var t in Advertisers)
            {
                if (t.ShowAd(adType))
                    return;
            }

            if (adType == AdType.Rewarded || adType == AdType.RewardedInterstitial)
                BaseAdHandler.AdNotAvailble();
        }
    }
}