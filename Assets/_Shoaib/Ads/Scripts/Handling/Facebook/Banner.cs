#if Facebook
using SH.Ads.Base;
using AudienceNetwork;
using UnityEngine;

namespace SH.Ads.Facebook
{
    public class Banner : BaseAdHandler
    {
        AdView adInstance;
        protected internal override bool IsAdAvailable => IsIntialized && adInstance != null && adInstance.IsValid();

        AdType adType;
        internal override void Intialize(AD ad)
        {
            IDs = ad.adIds;
            IsIntialized = true;
            adType = ad.adType;
            Debug.Log(this + " is intialized with " + IDs.Count + " ad Ids");
            loadAfterClose = ad.loadAfterClose;
            if (ad.loadAtStart)
                Load();
        }
        protected override void Load()
        {
            if (adLoading || !IsIntialized)
                return;

            if (IDs.Count > 0)
            {
                Remove();

                if(adType == AdType.Banner)
                       adInstance = new AdView(IDs[count], AdSize.BANNER_HEIGHT_50);
                else
                    adInstance = new AdView(IDs[count], AdSize.RECTANGLE_HEIGHT_250);

                adInstance.Register(new GameObject("Handler Facebook Banner"));
                adInstance.AdViewDidLoad += () => { adLoading = false; };
                adInstance.AdViewDidFailWithError += (error) =>
                {
                    adLoading = false;
                    Debug.Log($"Ad log : {this} Failed :  {count} cause : {error}");
                    if (count + 1 < IDs.Count)
                    {
                        count++;
                        Load();
                        return;
                    }
                    count = 0;
                };
                adInstance.AdViewWillLogImpression += () => { Debug.Log($"Ad log : {this} impression loged of ad id : {count}"); };
                adInstance.AdViewDidClick += () => { Debug.Log($"Ad log : {this} click impression of ad id : {count}"); };
                adLoading = true;
            }
        }

        internal override void Hide()
        {
            if (adInstance != null)
                adInstance.Dispose();
        }
        internal override void Remove()
        {
            if (adInstance != null)
                adInstance.Dispose();
        }
        internal override void Show()
        {
            if (IDs.Count == 0)
                return;
            if (IsAdAvailable)
            {
                if(adType == AdType.Banner)
                    adInstance.Show(AdPosition.TOP);
                else
                    adInstance.Show(0,0);
            }
            else
            {
                Load();
            }
        }


    }
}
#endif