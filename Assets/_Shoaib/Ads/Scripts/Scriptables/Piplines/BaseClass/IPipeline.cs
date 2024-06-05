
namespace SH.Ads.Base
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class IPipeline : ScriptableObject
    {
        [HideInInspector]public List<Advertiser> Advertisers = new List<Advertiser>();

#if UNITY_EDITOR
        [NonSerialized] public bool Folded = false;
        [field: SerializeField] public abstract string Name { get; }
        [field: SerializeField] public abstract string Description { get; }
#endif
        public IEnumerator Initialize()
        {
            foreach (var t in Advertisers)
            {
                Debug.Log("Ad status : Intializing advertiser : " + t.advertiser);
                yield return t.Initialize();
                Debug.Log($"Ad status : Advertiser '{t.advertiser}' Intialized");
            }
        }
        public virtual void ShowAd(AdType adType)
        {
            foreach (var t in Advertisers)
            {
                if (t.ShowAd(adType))
                    return;
            }

            if (adType == AdType.Rewarded || adType == AdType.RewardedInterstitial)
                BaseAdHandler.AdNotAvailble();
        }

        public void RemoveAd(AdType adType)
        {
            foreach (var t in Advertisers)
            {
                t.RemoveAd(adType);
            }
        }

        public void CopyValues(IPipeline copyfrom)
        {
            Advertisers= copyfrom.Advertisers;
        }
    }
}
