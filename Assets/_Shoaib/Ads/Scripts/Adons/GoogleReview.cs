#if GoogleReview
using Google.Play.Review;
using System;
using System.Collections;
using UnityEngine;

namespace SH.Ads.Adons
{
    public class GoogleReview : AdOn
    {
#if UNITY_EDITOR
        const string Location = "Assets/_Shoaib/Ads/Data/GoogleReview.asset";
        public static GoogleReview Load()
        {
            var remoteConfig = UnityEditor.AssetDatabase.LoadAssetAtPath<GoogleReview>(Location);
            if (remoteConfig == null)
            {
                remoteConfig = CreateInstance<GoogleReview>();
                UnityEditor.AssetDatabase.CreateAsset(remoteConfig, Location);
                UnityEditor.AssetDatabase.SaveAssets();
            }
            return remoteConfig;
        }
#endif
        internal override IEnumerator Initialize(AdSettings setting)
        {
            yield return null;
        }

        internal void RequestRateUs(Action<bool> Success)
        {
            ReviewManager _reviewManager = new ReviewManager();
            var requestFlowOperation = _reviewManager.RequestReviewFlow();
            requestFlowOperation.Completed += (v) =>
            {
                if (v.Error != ReviewErrorCode.NoError)
                {
                    Debug.LogError("Adon Status : Unable complete google Rate us flow");
                    Success?.Invoke(false);
                    return;
                }
                Success.Invoke(true);
            };

        }
    }
}
#endif