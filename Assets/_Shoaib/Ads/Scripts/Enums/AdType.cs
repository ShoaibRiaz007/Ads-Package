namespace SH.Ads
{
    [System.Serializable]
    public enum AdType
    {
        Banner=0,
        BigBanner=1,//admob only
        Interstitial = 2,
        RewardedInterstitial=3,//admon only
        Rewarded=4,
        OpenAd=5//ad mob only
    }
}