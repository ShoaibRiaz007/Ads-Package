#if Unity
using SH.Ads.Base;
using UnityEngine;
using UnityEngine.Advertisements;

namespace SH.Ads.Unity
{
    public class Interstitial : BaseAdHandler, IUnityAdsLoadListener, IUnityAdsShowListener
    {
        bool adLoaded = false;
        protected internal override bool IsAdAvailable => IsIntialized  && adLoaded;
        protected internal override bool IsAdShowing { get; protected set; }
        internal override void Intialize(AD ad)
        {
            IDs = ad.ADIds;
            IsIntialized = true;
            Debug.Log(this + " is intialized with "+  IDs.Count + " ad Ids");
            loadAfterClose = ad.LoadAfterClose;
            if (ad.LoadAtStart)
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
            IsAdShowing = false;

        }
        internal override void Remove()
        {
            IsAdShowing = false;
        }
        internal override void Show()
        {
            if ( IDs.Count == 0)
                return;
            if (IsAdAvailable)
            {
                Advertisement.Show(IDs[count], this);
                LocalAdShown = true;
                IsAdShowing = true;
            }
            else
                Load();
        }

        public void OnUnityAdsAdLoaded(string placementId)
        {
           adLoaded = true;
           adLoading = false;
            Debug.Log($"Ad log : {this} ad loaded :  {count}" + adLoaded);
            AdsManager.LogAnalyticEvent(this.ToString(), "On_Load", count.ToString());
        }

        public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
        {
            Debug.Log($"Ad log : {this} Failed :  {count} cause : {message}");
            AdsManager.LogAnalyticEvent(this.ToString(), "On_Fail",message);
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
            IsAdShowing = true;
        }

        public void OnUnityAdsShowClick(string placementId)
        {
            Debug.Log($"Ad log : {this} shown click :  {count}");
        }

        public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
        {
            Debug.Log($"Ad log : {this} shown complete :  {count}");
            IsAdShowing = false;
            if (loadAfterClose)
                Load();
        }
    }
}
#endif