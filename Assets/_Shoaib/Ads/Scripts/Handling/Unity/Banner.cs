#if Unity
using SH.Ads.Base;
using UnityEngine;
using UnityEngine.Advertisements;

namespace SH.Ads.Unity
{
    public class Banner : BaseAdHandler
    {
        protected internal override bool IsAdAvailable => IsIntialized && Advertisement.Banner.isLoaded ;
        protected internal override bool IsAdShowing { get; protected set; }
        internal override void Intialize(AD ad)
        {
            IDs = ad.ADIds;
            IsIntialized = true;
            Debug.Log(this + " is intialized with " + IDs.Count + " ad Ids");
            loadAfterClose = ad.LoadAfterClose;
            if (ad.LoadAtStart)
                Load();
        }
        protected override void Load()
        {
            if (adLoading || !IsIntialized)
                return;

            if ( IDs.Count > 0)
            {
                Remove();
                Advertisement.Banner.SetPosition(UnityEngine.Advertisements.BannerPosition.TOP_CENTER);
                Advertisement.Banner.Show(IDs[count], new BannerOptions() { showCallback = () => { adLoading = false; IsAdShowing = true; } });
                adLoading = true;
            }
        }
        internal override void Hide()
        {
            if (IsAdAvailable)
                Advertisement.Banner.Hide(false);
            IsAdShowing = false;

        }
        internal override void Remove()
        {
            if (IsAdAvailable)
                Advertisement.Banner.Hide(true);
            IsAdShowing = false;
        }
        internal override void Show()
        {
            if ( IDs.Count == 0)
                return;

            if (IsAdAvailable)
            {
                Advertisement.Banner.SetPosition(UnityEngine.Advertisements.BannerPosition.TOP_CENTER);
                Advertisement.Banner.Show(IDs[count]);
                IsAdShowing = true;
            }
            else
                Load();
        }
    }
}
#endif