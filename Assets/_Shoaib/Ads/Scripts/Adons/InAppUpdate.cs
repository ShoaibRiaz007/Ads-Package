#if InAppUpdate
using Google.Play.AppUpdate;
using Google.Play.Common;
using System.Collections;
using UnityEngine;

namespace SH.Ads.Adons
{
    public class InAppUpdate : AdOn
    {
#if UNITY_EDITOR
        const string Location = "Assets/_Shoaib/Ads/Data/InAppUpdate.asset";
        public static InAppUpdate Load()
        {
            var InAppUpdate = UnityEditor.AssetDatabase.LoadAssetAtPath<InAppUpdate>(Location);
            if (InAppUpdate == null)
            {
                InAppUpdate = CreateInstance<InAppUpdate>();
                UnityEditor.AssetDatabase.CreateAsset(InAppUpdate, Location);
                UnityEditor.AssetDatabase.SaveAssets();
            }
            return InAppUpdate;
        }
#endif

        internal override IEnumerator Initialize(AdSettings setting)
        {
            if (Application.isEditor)
                yield break;

            AppUpdateManager appUpdateManager = new AppUpdateManager();
            yield return CheckForUpdate(appUpdateManager);
        }

        IEnumerator CheckForUpdate(AppUpdateManager appUpdateManager)
        {
            PlayAsyncOperation<AppUpdateInfo, AppUpdateErrorCode> appUpdateInfoOperation =
              appUpdateManager.GetAppUpdateInfo();

            yield return appUpdateInfoOperation;

            if (appUpdateInfoOperation.IsSuccessful)
            {
                var appUpdateInfoResult = appUpdateInfoOperation.GetResult();
                if(appUpdateInfoResult.AppUpdateStatus == AppUpdateStatus.Pending)
                {
                    Debug.LogError("Update Pirority : "+appUpdateInfoResult.UpdatePriority);
                    if(appUpdateInfoResult.UpdatePriority < 9)
                    {
                        yield break;
                    }
                    else if(appUpdateInfoResult.UpdatePriority < 50 )
                    {
                        //user may skip the update and run in BG
                        var stalenessDays = appUpdateInfoResult.ClientVersionStalenessDays;
                        if(stalenessDays!=null && stalenessDays > 1)
                        {
                            yield return StartFlexibleUpdate(appUpdateManager, appUpdateInfoResult);
                        }

                    }
                    else
                    {
                        yield return StartImmediateUpdate(appUpdateManager, appUpdateInfoResult);
                    }
                }
            }
            else
            {
                Debug.LogError($"{this} : Unable to load update information /n Error [{appUpdateInfoOperation.Error.ToString()}]",this);
            }
        }

        IEnumerator StartFlexibleUpdate(AppUpdateManager appUpdateManager, AppUpdateInfo appUpdateInfoResult)
        {
            var startUpdateRequest = appUpdateManager.StartUpdate(
              appUpdateInfoResult,
              AppUpdateOptions.FlexibleAppUpdateOptions(allowAssetPackDeletion: true));

            while (!startUpdateRequest.IsDone)
            {
                Debug.Log($"{this} : Downloading {startUpdateRequest.DownloadProgress} %");
                yield return null;
            }

            var result = appUpdateManager.CompleteUpdate();
            yield return result;
            Debug.LogError($"{this} : Unable to update the application. \n Error [{startUpdateRequest.Error.ToString()}]", this);
        }
        IEnumerator StartImmediateUpdate(AppUpdateManager appUpdateManager, AppUpdateInfo appUpdateInfoResult)
        {
            var startUpdateRequest = appUpdateManager.StartUpdate(
              appUpdateInfoResult,
              AppUpdateOptions.ImmediateAppUpdateOptions(allowAssetPackDeletion: true));
            yield return startUpdateRequest;
            Debug.LogError($"{this} : Unable to update the application. \n Error [{startUpdateRequest.Error.ToString()}]", this);
        }


    }
}
#endif