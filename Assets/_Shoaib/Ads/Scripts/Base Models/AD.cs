using System;
using System.Collections.Generic;
using UnityEngine;

namespace SH.Ads.Base
{
    [System.Serializable]
    public class AD
    {
        [SerializeField] public AdType type = AdType.Banner;
        [SerializeField] public List<string> ADIds = new List<string>();
        [SerializeField] public bool LoadAtStart=false,LoadAfterClose=false;
        [SerializeField] public float ADReshowTime=0;
        private BaseAdHandler adHandler;
#if UNITY_EDITOR
        [NonSerialized]public bool Folded = false;
#endif
        public void Initialize(SupportedAdvertisers advertiser)
        {
            if (adHandler == null)
            {
                Type adType = Type.GetType("SH.Ads." + advertiser + "." + type.ToString().Replace("Big", string.Empty));
                if (adType != null)
                {
                    adHandler = Activator.CreateInstance(adType) as BaseAdHandler;
                    adHandler.Initialize(this);
                }
                else
                    Debug.LogError("No type found. Finding type {"+ "SH.Ads." + advertiser + "." + type.ToString().Replace("Big", string.Empty)+"}");
               
            }
            else
                adHandler.Initialize(this);
        }
        public void ShowAd(AdType type)
        {
           

            if (adHandler == null)
            {
                Debug.LogError("AdHandler is not initialized.");
                return;
            }
            if (type == AdType.Banner && this.type == AdType.BigBanner)
            {
                adHandler.Remove();
            }
            else if (type == AdType.BigBanner && this.type == AdType.Banner)
            {
                adHandler.Remove();
            }
            if (type != this.type)
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
            if (type == AdType.Banner && this.type == AdType.BigBanner)
            {
                adHandler.Hide();
            }
            else if (type == AdType.BigBanner && this.type == AdType.Banner)
            {
                adHandler.Hide();
            }
            if (type != this.type)
                return;
            if(adHandler.IsAdShowing)
                adHandler.Hide();
        }

        public bool IsAvailable => adHandler==null? false: adHandler.IsAdAvailable;
    }
}