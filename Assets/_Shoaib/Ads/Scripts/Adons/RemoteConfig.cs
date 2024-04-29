#if RemoteConfig
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
        static RemoteConfig Instance = null;

        [HideInInspector]public RemoteData m_RemoteData;
        [HideInInspector] public string m_AdSettingKey= "AdSettings", m_DataKey = "Data";


        public static RemoteData Data
        {
            get
            {
                if (Instance == null)
                {
                    Debug.LogError("Ad Error : Initialize the Ads First then you can read these values");
                    return new RemoteData();
                }

                return Instance.m_RemoteData;
            }
        }

        internal override IEnumerator Initialize(AdSettings setting)
        {
            Instance = this;
            if (FirebaseRemoteConfig.DefaultInstance==null)
                yield return Firebase.FirebaseApp.CheckAndFixDependenciesAsync();
            
            Dictionary<string, object> defaults = new Dictionary<string, object>
            {
                { m_AdSettingKey, "{}" },
                { m_DataKey, "{}" },
            };

            yield return FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults)
              .ContinueWith(result => FirebaseRemoteConfig.DefaultInstance.FetchAndActivateAsync())
              .Unwrap();

            var json = FirebaseRemoteConfig.DefaultInstance.GetValue(m_AdSettingKey).StringValue;
            if (json == "{}")// check if json is null
            {
                Debug.Log("Ad Status : Default ad setting key not found");
                yield break;
            }
           
            JsonUtility.FromJsonOverwrite(json, setting.CurrentPipline);
            json = FirebaseRemoteConfig.DefaultInstance.GetValue(m_DataKey).StringValue;
            if (json == "{}")// check if json is null
            {
                Debug.Log("Ad Status : Data key not not found");
                yield break;
            }
            JsonUtility.FromJsonOverwrite(json, m_RemoteData);
            yield return null;// now load advertisers after one frame
        }
    }
}
#endif