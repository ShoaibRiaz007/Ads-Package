using System;
using System.Collections.Generic;
using UnityEngine;

namespace SH.Ads.Base
{
    [System.Serializable]
    public class AD
    {
        [SerializeField] public AdType adType = AdType.Banner;
        [SerializeField] public List<string> adIds = new List<string>();
        [SerializeField] public bool loadAtStart=false,loadAfterClose=false;
        private BaseAdHandler adHandler;
#if UNITY_EDITOR
        [NonSerialized]public bool Folded = false;
#endif
        public void Intialize(SupportedAdvertisers advertiser)
        {
            if (adHandler == null)
            {
                Type type = Type.GetType("SH.Ads." + advertiser + "." + adType.ToString().Replace("Big", string.Empty));
                if (type != null)
                {
                    adHandler = Activator.CreateInstance(type) as BaseAdHandler;
                    adHandler.Intialize(this);
                }
                else
                    UnityEngine.Debug.LogError("No type found. Finding type {"+ "SH.Ads." + advertiser + "." + adType.ToString().Replace("Big", string.Empty)+"}");
               
            }
            else
                adHandler.Intialize(this);
        }
        public void ShowAd(AdType type)
        {
           

            if (adHandler == null)
            {
                Debug.LogError("AdHandler is not initialized.");
                return;
            }
            if (type == AdType.Banner && adType == AdType.BigBanner)
            {
                adHandler.Hide();
            }
            else if (type == AdType.BigBanner && adType == AdType.Banner)
            {
                adHandler.Hide();
            }
            if (type != adType)
                return;
            adHandler.Show();
        }

        internal void RemoveAd(AdType type)
        {
            if (adHandler == null)
            {
                Debug.LogError("AdHandler is not initialized.");
                return;
            }
            if (type != adType)
                return;

            adHandler.Remove();
        }

        public bool IsAvailable => adHandler.IsAdAvailable;
    }
}