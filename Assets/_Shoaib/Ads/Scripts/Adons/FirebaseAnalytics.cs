#if FirebaseAnalytics
using Firebase;
using Firebase.Analytics;
using Firebase.Extensions;
using System.Collections;
using UnityEngine;

namespace SH.Ads.Adons
{
    public class FirebaseAnalytics : AdOn
    {
#if UNITY_EDITOR
        const string Location = "Assets/_Shoaib/Ads/Data/FirebaseAnalytics.asset";
        public static FirebaseAnalytics Load()
        {
            var remoteConfig = UnityEditor.AssetDatabase.LoadAssetAtPath<FirebaseAnalytics>(Location);
            if (remoteConfig == null)
            {
                remoteConfig = CreateInstance<FirebaseAnalytics>();
                UnityEditor.AssetDatabase.CreateAsset(remoteConfig, Location);
                UnityEditor.AssetDatabase.SaveAssets();
            }
            return remoteConfig;
        }
#endif
        internal override IEnumerator Initialize(AdSettings setting)
        {
            bool initalized = false;

            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
                var dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available)
                {
                    Debug.Log("Ad Log : Enabling data collection.");
                    Firebase.Analytics.FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
                    Firebase.Analytics.FirebaseAnalytics.SetSessionTimeoutDuration(new System.TimeSpan(0, 30, 0));
                    initalized = true;
                }
                else
                {
                    Debug.LogError("Ad Error : Could not resolve all Firebase dependencies: " + dependencyStatus);
                }
            });
            while (!initalized)
                yield return null;
        }

        internal void LogEvent(string eventName, string parameterName, string parameterValue)
        {
            Firebase.Analytics.FirebaseAnalytics.LogEvent(eventName, parameterName, parameterValue);
        }

        internal void LogEvent(string name)
        {
            Firebase.Analytics.FirebaseAnalytics.LogEvent(name);
        }

        internal void LogEvent(string name, params Parameter[] parameters)
        {
            Firebase.Analytics.FirebaseAnalytics.LogEvent(name, parameters);
        }
    }
}
#endif