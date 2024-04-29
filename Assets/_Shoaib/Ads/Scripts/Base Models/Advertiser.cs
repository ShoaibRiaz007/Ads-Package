using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SH.Ads.Base
{
    [System.Serializable]
    public class  Advertiser
    {
        [SerializeField] public SupportedAdvertisers advertiser;
        [SerializeField] public string ID;
        [SerializeField] public bool IsAndroid;
        [SerializeField] public bool IsIOS;

        [SerializeField] public List<AD> Ads = new List<AD>();
        [SerializeField] public int order;

#if UNITY_EDITOR
        public bool Folded { get => UnityEditor.EditorPrefs.GetBool($"Floded_{advertiser}"); set=>UnityEditor.EditorPrefs.SetBool($"Floded_{advertiser}",value);}
#endif


        BaseManager manager;
        public IEnumerator Initialize()
        {
            if (AdSettings.RemoveAd)
                yield break;

            if (manager == null)
            {
                Type type = Type.GetType("SH.Ads." + advertiser + ".Manager");
                if (type != null)
                {
                    manager = Activator.CreateInstance(type) as BaseManager;
                   yield return manager.Initialize(this, AdSettings.IsForChildren,AdSettings.AgeGroupRating);
                }
                else
                    Debug.LogError("No type found. Finding type {" + "SH.Ads." + advertiser + ".Manager}");
            }
            else
            {
                foreach (var t in Ads)// Intialize ID => for spiral pipline
                    t.Initialize(advertiser);
            }
        }


        public bool Intialized => manager != null;

        public bool ShowAd(AdType type)
        {
            if (AdSettings.RemoveAd)
                return true;
            var available = IsAdAvailable(type);
            foreach (var tem in Ads)
                 tem.ShowAd(type);
            return available;
        }

        public void RemoveAd(AdType type)
        {
            foreach (var tem in Ads)
                tem.RemoveAd(type);
        }
        bool IsAdAvailable(AdType type)
        {
            for(int i = 0; i < Ads.Count; i++)
            {
                if (Ads[i].type == type)
                    return Ads[i].IsAvailable;
            }

            return false;
        }

    }
}