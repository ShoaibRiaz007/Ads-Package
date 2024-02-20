#if RemoteConfig
using Firebase.Extensions;
using Firebase.RemoteConfig;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SH.Ads.Adons
{
    public class RemoteConfig : AdOn
    {
#if UNITY_EDITOR
        const string Location = "Assets/_Shoaib/Ads/Data/RemoteConfig.asset";
        public static RemoteConfig Load()
        {
            var remoteConfig = UnityEditor.AssetDatabase.LoadAssetAtPath<RemoteConfig>(Location);
            if (remoteConfig == null)
            {
                remoteConfig = CreateInstance<RemoteConfig>();
                UnityEditor.AssetDatabase.CreateAsset(remoteConfig, Location);
                UnityEditor.AssetDatabase.SaveAssets();
            }
            return remoteConfig;
        }
#endif

        const string AD_SETTINGS = "AdSettings";

        internal override IEnumerator Intialize(AdSettings setting)
        {
            if(FirebaseRemoteConfig.DefaultInstance==null)
                yield return Firebase.FirebaseApp.CheckAndFixDependenciesAsync();
            Dictionary<string, object> defaults = new Dictionary<string, object>
            {
                { AD_SETTINGS, "{}" }
            };

            yield return FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults)
              .ContinueWith(result => FirebaseRemoteConfig.DefaultInstance.FetchAndActivateAsync())
              .Unwrap();
            var json = FirebaseRemoteConfig.DefaultInstance.GetValue(AD_SETTINGS).StringValue;
            if (json == "{}")// check if json is null
            {
                Debug.Log("Ad Status : Remote config loaded but is empty,so Ignoring it");
                yield break;
            }
            JsonUtility.FromJsonOverwrite(json, setting.CurrentPipline);
            yield return null;// now load advertisers after one frame
        }
    }
}
#endif