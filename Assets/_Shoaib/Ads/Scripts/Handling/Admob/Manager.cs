#if Admob
using GoogleMobileAds.Api;
using GoogleMobileAds.Ump.Api;
using SH.Ads.Base;
using System.Collections;
using UnityEngine;

namespace SH.Ads.Admob
{
    /// <summary>
    /// Manager for Admob
    /// </summary>
    internal class Manager : BaseManager
    {
        bool loadingConcent = true;
        internal override IEnumerator Initialize(Base.Advertiser advertiser, bool isForChildren, string ageGroup)
        {
            yield return IsNetworkAvailable();

            TagForChildDirectedTreatment tagFororChildren;
            TagForUnderAgeOfConsent tagForUnderAge;
            MaxAdContentRating contentRating;

            if (isForChildren)
            {
                tagFororChildren = TagForChildDirectedTreatment.True;
                tagForUnderAge = TagForUnderAgeOfConsent.True;
                contentRating = MaxAdContentRating.G;
            }
            else
            {
                tagFororChildren = TagForChildDirectedTreatment.Unspecified;
                tagForUnderAge = TagForUnderAgeOfConsent.Unspecified;
                contentRating = MaxAdContentRating.ToMaxAdContentRating(ageGroup);
            }
            SetupUMPForm();
            while (loadingConcent)
                yield return null;

            if(ConsentInformation.ConsentStatus == ConsentStatus.Required && ConsentInformation.ConsentStatus != ConsentStatus.Obtained)
            {
                Debug.LogError("Ad Status : UMP concent required but didn't obtained");
            }

            var requestConfiguration = new RequestConfiguration();
            requestConfiguration.TagForChildDirectedTreatment = tagFororChildren;
            requestConfiguration.TagForUnderAgeOfConsent = tagForUnderAge;
            requestConfiguration.MaxAdContentRating = contentRating;
            // Add test device ID.
            if (AdSettings.TestMode)
            {
                if (!AdSettings.TestDevices.Contains(AdRequest.TestDeviceSimulator))
                    AdSettings.TestDevices.Add(AdRequest.TestDeviceSimulator);
                if (!AdSettings.TestDevices.Contains(SystemInfo.deviceUniqueIdentifier.ToUpper().Trim()))
                    AdSettings.TestDevices.Add(SystemInfo.deviceUniqueIdentifier.ToUpper().Trim());
            }
            requestConfiguration.TestDeviceIds = AdSettings.TestDevices;
            MobileAds.SetRequestConfiguration(requestConfiguration);
            MobileAds.SetiOSAppPauseOnBackground(true);

            ApplyMediationSettings();


            bool completed = false;
            MobileAds.Initialize((status) =>
            {
                foreach (var map in status.getAdapterStatusMap())
                {
                    switch (map.Value.InitializationState)
                    {
                        case AdapterState.NotReady:
                            // The adapter initialization did not complete.
                            Debug.LogFormat("Ad Status : Adapter: {0} : {1} not ready.", map.Key, map.Value);
                            break;
                        case AdapterState.Ready:
                            // The adapter was successfully initialized.
                            Debug.LogFormat("Ad Status : Adapter: {0} : {1} ready.", map.Key, map.Value);
                            break;
                    }
                }
                completed=true;
                foreach (var tem in advertiser.Ads)
                    tem.Initialize(advertiser.advertiser);
            });

            while (!completed)
                yield return null;
        }

        private void ApplyMediationSettings()
        {
#if MediationAdmobUnity
            GoogleMobileAds.Mediation.UnityAds.Api.UnityAds.SetConsentMetaData("gdpr.consent", AdSettings.GDPRConcent);
            GoogleMobileAds.Mediation.UnityAds.Api.UnityAds.SetConsentMetaData("privacy.consent", AdSettings.CCPAConsent);
#endif
#if MediationAdmobAppLovin
            GoogleMobileAds.Mediation.AppLovin.Api.AppLovin.SetHasUserConsent(AdSettings.CCPAConsent);
            GoogleMobileAds.Mediation.AppLovin.Api.AppLovin.SetIsAgeRestrictedUser(AdSettings.IsForChildren);
            GoogleMobileAds.Mediation.AppLovin.Api.AppLovin.SetDoNotSell(AdSettings.GDPRConcent);
#endif
        }

        // Start is called before the first frame update

        void SetupUMPForm()
        {
            try
            {
                ConsentRequestParameters request = null;
                if (AdSettings.TestMode)
                {
                    var debugSettings = new ConsentDebugSettings
                    {
                        DebugGeography = DebugGeography.EEA,
                        TestDeviceHashedIds = AdSettings.TestDevices,
                    };
                    request = new ConsentRequestParameters
                    {
                        TagForUnderAgeOfConsent = AdSettings.IsForChildren,
                        ConsentDebugSettings = debugSettings
                    };
                }
                else
                {
                    request = new ConsentRequestParameters
                    {
                        TagForUnderAgeOfConsent = AdSettings.IsForChildren,
                    };
                }
                ConsentInformation.Update(request, OnConsentInfoUpdated);
            } catch(System.Exception e)
            {
                Debug.LogError("Ads Log: Unable to start ump form. Check google console to add UMP in the game :\n" + e.Message);
                loadingConcent = false;
            }

        }


        void OnConsentInfoUpdated(FormError error)
        {
            if (error != null)
            {
                Debug.LogError(error);
                return;
            }
            if (ConsentInformation.IsConsentFormAvailable())
                ConsentForm.Load(OnLoadConsentForm);
            else
                loadingConcent = false;
        }

        private void OnLoadConsentForm(ConsentForm form, FormError error)
        {
            if (error != null)
            {
                Debug.LogError("Ad Status : Error : " + error.Message);
                return;
            }
            if (ConsentInformation.ConsentStatus == ConsentStatus.Required)
                form.Show(OnShowForm);
            else
                loadingConcent = false;
        }

        void OnShowForm(FormError error)
        {
            if (error != null)
            {
                Debug.LogError("Ad Status : Error : "+error.Message);
                return;
            }
            loadingConcent = false;
            Debug.Log("Ad Status : UMP form updated. Status : " + ConsentInformation.ConsentStatus );
        }
    }
    
}
#endif