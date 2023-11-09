
namespace SH.Ads.Base
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class IPipeline : ScriptableObject
    {
        [field: SerializeField] public virtual string Name { get; }
        [field: SerializeField] public virtual string Description { get; }
        [SerializeField] public List<Advertiser> Advertisers = new List<Advertiser>();

#if UNITY_EDITOR
        [NonSerialized] public bool Folded = false;
#endif
        public virtual IEnumerator Intialize() { yield break; }
        public virtual void ShowAd(AdType adType) { }

        public void CopyValues(IPipeline copyfrom )
        {
            Advertisers= copyfrom.Advertisers;
        }
    }
}
