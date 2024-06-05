#if Facebook
using SH.Ads.Base;
using AudienceNetwork;
using UnityEngine;

namespace SH.Ads.Facebook
{
    public class Interstitial : BaseAdHandler
    {
        InterstitialAd adInstance;
        protected internal override bool IsAdAvailable => IsIntialized  && adInstance != null && adInstance.IsValid();
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
                adInstance = new InterstitialAd(IDs[count]);
                adInstance.Register(new GameObject("Handler Facebook Interstial"));

                adInstance.InterstitialAdDidLoad += ()=> { adLoading = false; AdsManager.LogAnalyticEvent(this.ToString(), "On_Load", count.ToString()); };
                adInstance.InterstitialAdDidFailWithError += (error)=> 
                { 
                    adLoading = false;
                    Debug.Log($"Ad log : {this} Failed :  {count} cause : {error}");
                    AdsManager.LogAnalyticEvent(this.ToString(), "On_Fail", error);
                    if (count + 1 < IDs.Count)
                    {
                        count++;
                        Load();
                        return;
                    }
                    count = 0;
                    IsAdShowing = false;
                };
                adInstance.InterstitialAdWillLogImpression += ()=> { Debug.Log($"Ad log : {this} impression loged of ad id : {count}"); };
                adInstance.InterstitialAdDidClick += () => { Debug.Log($"Ad log : {this} click impression of ad id : {count}"); };
                adInstance.InterstitialAdDidClose += () => 
                { 
                    Debug.Log($"Ad log : {this} close impression of ad id : {count}");
                    IsAdShowing = false;
                    if (loadAfterClose)
                        Load();
                };
#if UNITY_ANDROID
                /*
                 * Only relevant to Android.
                 * This callback will only be triggered if the Interstitial activity has
                 * been destroyed without being properly closed. This can happen if an
                 * app with launchMode:singleTask (such as a Unity game) goes to
                 * background and is then relaunched by tapping the icon.
                 */
                adInstance.interstitialAdActivityDestroyed =()=>
                {
                    IsAdShowing = false;
                    Remove();
                };
#endif
                adLoading = true;
            }
        }

        internal override void Hide()
        {
            if (adInstance != null)
                adInstance.Dispose();
            IsAdShowing = false;
        }
        internal override void Remove()
        {
            if (adInstance != null)
                adInstance.Dispose();
            IsAdShowing = false;
        }
        internal override void Show()
        {
            if ( IDs.Count == 0)
                return;
            if (IsAdAvailable)
            {
                IsAdShowing = true;
                LocalAdShown = true;
                adInstance.Show();
            }
            else
            {
               Load();
            }
        }


    }
}
#endif