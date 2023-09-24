#if Facebook
using SH.Ads.Base;
using AudienceNetwork;
using UnityEngine;

namespace SH.Ads.Facebook
{
    public class Interstial : BaseAdHandler
    {
        InterstitialAd adInstance;
        protected internal override bool IsAdAvailable => IsIntialized  && adInstance != null && adInstance.IsValid();
        internal override void Intialize(AD ad)
        {
            IDs = ad.adIds;
            IsIntialized = true;

            Debug.Log(this + " is intialized with " + IDs.Count + " ad Ids");
            loadAfterClose = ad.loadAfterClose;
            if (ad.loadAtStart)
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

                adInstance.InterstitialAdDidLoad += ()=> { adLoading = false; };
                adInstance.InterstitialAdDidFailWithError += (error)=> 
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
                adInstance.InterstitialAdWillLogImpression += ()=> { Debug.Log($"Ad log : {this} impression loged of ad id : {count}"); };
                adInstance.InterstitialAdDidClick += () => { Debug.Log($"Ad log : {this} click impression of ad id : {count}"); };
                adInstance.InterstitialAdDidClose += () => 
                { 
                    Debug.Log($"Ad log : {this} close impression of ad id : {count}"); 
                    if(loadAfterClose)
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
        }
        internal override void Remove()
        {
            if (adInstance != null)
                adInstance.Dispose();
        }
        internal override void Show()
        {
            if ( IDs.Count == 0)
                return;
            if (IsAdAvailable)
            {
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