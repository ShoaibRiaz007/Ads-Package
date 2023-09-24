#if Unity
using SH.Ads.Base;
using UnityEngine;
using UnityEngine.Advertisements;

namespace SH.Ads.Unity
{
    public class Interstial : BaseAdHandler, IUnityAdsLoadListener, IUnityAdsShowListener
    {
        bool adLoaded = false;
        protected internal override bool IsAdAvailable => IsIntialized  && adLoaded;
        internal override void Intialize(AD ad)
        {
            IDs = ad.adIds;
            IsIntialized = true;
            Debug.Log(this + " is intialized with "+  IDs.Count + " ad Ids");
            loadAfterClose = ad.loadAfterClose;
            if (ad.loadAtStart)
                Load();
        }
        protected override void Load()
        {
            if (adLoading || Advertisement.isShowing || !IsIntialized)
                return;

            if ( IDs.Count > 0)
            {
                Remove();
                Advertisement.Load(IDs[count], this);
                adLoading = true;
            }
        }
        internal override void Hide()
        {
            

        }
        internal override void Remove()
        {
        }
        internal override void Show()
        {
            if ( IDs.Count == 0)
                return;
            if (IsAdAvailable)
            {
                Advertisement.Show(IDs[count], this);
               
            }
            else
                Load();
        }

        public void OnUnityAdsAdLoaded(string placementId)
        {
           adLoaded = true;
           adLoading = false;
            Debug.Log($"Ad log : {this} ad loaded :  {count}" + adLoaded);
        }

        public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
        {
            Debug.Log($"Ad log : {this} Failed :  {count} cause : {message}");
            adLoading = false;
            if (count + 1 < IDs.Count)
            {
                count++;
                Load();
                return;
            }
            count = 0;
            return;
        }

        public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
        {
            Debug.Log($"Ad log : {this} shown falure :  {count} cause : {message}");
        }

        public void OnUnityAdsShowStart(string placementId)
        {
            Debug.Log($"Ad log : {this} shown scuess :  {count}");
        }

        public void OnUnityAdsShowClick(string placementId)
        {
            Debug.Log($"Ad log : {this} shown click :  {count}");
        }

        public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
        {
            Debug.Log($"Ad log : {this} shown complete :  {count}");
            if (loadAfterClose)
                Load();
        }
    }
}
#endif