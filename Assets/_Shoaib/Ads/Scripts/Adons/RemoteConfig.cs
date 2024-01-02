#if RemoteConfig
using Firebase.Extensions;
using Firebase.RemoteConfig;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SH.Ads.Piplines;

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
            bool initalized = false;
            Dictionary<string, object> defaults = new Dictionary<string, object>();
            defaults.Add(AD_SETTINGS, "{}");

            var remoteConfig = FirebaseRemoteConfig.DefaultInstance;
            remoteConfig.SetDefaultsAsync(defaults)
              .ContinueWith(result => remoteConfig.FetchAndActivateAsync())
              .Unwrap()
              .ContinueWithOnMainThread(task => {
                  Debug.Log("Ad Status : Remote Config is intialized");
                  initalized = true;

                  var json = FirebaseRemoteConfig.DefaultInstance.GetValue(AD_SETTINGS).StringValue;
                  if (json == "{}")// check if json is null
                  {
                      Debug.Log("Ad Status : Remote config loaded but is empty,so Ignoring it");
                      return;
                  }
                  JsonUtility.FromJsonOverwrite(json, setting.CurrentPipline);
              });

            while (!initalized)
                yield return null;

            yield return null;// now load advertisers after one frame
        }
    }
}
#endif