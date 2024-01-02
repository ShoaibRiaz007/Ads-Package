using SH.Ads.Base;

namespace SH.Ads.Piplines
{
    public class Waterfall : IPipeline
    {
#if UNITY_EDITOR
        public override string Name => "Waterfall Pipline";
        public override string Description =>
            "The Waterfall Pipeline optimizes ad delivery by prioritizing advertisers in sequence. " +
            "\n\nFor example, you can configure it to display ads from AdMob, " +
            "and if unavailable, it will fall back to Unity or any other advertiser you added" +
            "\n\nThis Waterfall Pipeline allows efficient utilization of multiple ad platforms, " +
            "ensuring ads are served from the first available advertiser for each ad type.";

        const string Location = "Assets/_Shoaib/Ads/Data/Waterfall.asset";

        public static Waterfall Load()
        {
            var customWaterfallPipline = UnityEditor.AssetDatabase.LoadAssetAtPath<Waterfall>(Location);
            if (customWaterfallPipline == null)
            {
                customWaterfallPipline = CreateInstance<Waterfall>();
                UnityEditor.AssetDatabase.CreateAsset(customWaterfallPipline, Location);
                UnityEditor.AssetDatabase.SaveAssets();
            }
            return customWaterfallPipline;
        }
#endif
    }
}