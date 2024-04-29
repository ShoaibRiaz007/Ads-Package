#if UNITY_EDITOR
using SH.Ads.Editor.Adons;
using SH.Ads.Editor.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace SH.Ads.Editor
{
    public class InstallPackage : IWindow
    {
        const string DependenceisPath = "Assets/_Shoaib/Ads/Dependencies";
        const string DependenceisAdonsPath = "Assets/_Shoaib/Ads/Dependencies/Adons";
        const string UnityAd = "com.unity.ads";

        List<string> DependencieAdvertiserPackages = new List<string>(), DependenciesAdOnPackages = new List<string>();

        public Adon[] allAdons = new Adon[]
        {
             new FirebaseAnalytics(),
             new RemoteConfig(),
             new GoogleReview(),
        };

        ListRequest PackageManagerInstallRequestList;
        AddRequest PackageManagerInstallRequest;
        
        
        static Vector2 scrollPos;
        InstalledPackages installedPackagesFiles = null;

        public override string Name => "Package Installer";

        public override string ToolTip => "Panel to intall new advertiser";

        public InstallPackage()
        {
            AssetDatabase.importPackageCompleted += ImportPackageCompleted;
            AssetDatabase.importPackageCancelled += ImportPackageCanceled;
            AssetDatabase.importPackageStarted += ImportPackageStarted;
            AssetDatabase.importPackageFailed += ImportPackageFailed;
            AssetDatabase.onImportPackageItemsCompleted += ImportedItems;
        }

        public override void OnEnable(AdSettings settings)
        {
            Extensions.CheckForInstalledPackages();
            ListFilesInFolder();
            EditorUtility.SetDirty(settings);
            for (int i = 0; i < allAdons.Length; i++)
            {
                allAdons[i].CheckIfInstalled();
            }
            installedPackagesFiles =InstalledPackages.Load();
        }

        public override void OnGUI()
        {
            Header();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, new GUIStyle(EditorStyles.helpBox));
            if (string.IsNullOrEmpty(installedPackagesFiles.ActivePackage))
            {
                EditorGUILayout.LabelField("Packages : ", new GUIStyle(EditorStyles.boldLabel) { fontSize = 30, fixedHeight = 30 });
                EditorGUILayout.Space(30);
                foreach (var t in Enum.GetValues(typeof(SupportedAdvertisers)).Cast<SupportedAdvertisers>())
                    ShowAdvertiser(t);

                EditorGUILayout.Space(50);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("Ad Ons : ", new GUIStyle(EditorStyles.boldLabel) { fontSize=40, fixedHeight=50});
                EditorGUILayout.Space(30);
                for (int i = 0; i < allAdons.Length; i++)
                {
                    ShowOption(allAdons[i]);
                }

                EditorGUILayout.EndVertical();
            }
            else
            {
                EditorGUILayout.HelpBox("Importing package is in progress. Please wait for it complete. \n Some times it get stuck when you close it with X on rigt upper corner.", MessageType.Warning);
                if (GUILayout.Button(new GUIContent("Click Here if stuck"), new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleCenter, fixedHeight = 50 }))
                {
                    installedPackagesFiles.ActivePackage= string.Empty;
                }

            }
            EditorGUILayout.EndScrollView();
        }

        void ListFilesInFolder()
        {
            if (Directory.Exists(DependenceisPath) && Directory.Exists(DependenceisAdonsPath))
            {
                DependencieAdvertiserPackages = new List<string>();
                DependencieAdvertiserPackages.AddRange(Directory.GetFiles(DependenceisPath));
                DependencieAdvertiserPackages = DependencieAdvertiserPackages.Where(a => !a.Contains(".meta")).ToList();

                DependenciesAdOnPackages = new List<string>();
                DependenciesAdOnPackages.AddRange(Directory.GetFiles(DependenceisAdonsPath));
                DependenciesAdOnPackages = DependenciesAdOnPackages.Where(a => !a.Contains(".meta")).ToList();
            }
            else
                EditorUtility.DisplayDialog("Error", $"Dictionary  [{DependenceisPath}] or [{DependenceisAdonsPath}] not found. Please create folder and place dependent packages in it.", "OK");

        }

        void Header()
        {
            EditorGUILayout.HelpBox("This window is used to install the dependencies of Advertisment package. Bellow are all the supported and tested packages.", MessageType.Info);
            EditorGUILayout.Space();
        }
        void ShowAdvertiser(SupportedAdvertisers advertiser)
        {

            EditorGUILayout.LabelField(advertiser.ToString(), EditorStyles.boldLabel);
            switch (advertiser)
            {
                case SupportedAdvertisers.Admob:
                    foreach (var path in DependencieAdvertiserPackages)
                        if (path.Contains("GoogleMobileAds"))
                            ShowOption(path.Split(Path.DirectorySeparatorChar)[1], path, advertiser.IsInstalled());
                    return;
                case SupportedAdvertisers.AdColony:
                    foreach (var path in DependencieAdvertiserPackages)
                        if (path.Contains("AdColony"))
                            ShowOption(path.Split(Path.DirectorySeparatorChar)[1], path, advertiser.IsInstalled());
                    return;
                case SupportedAdvertisers.Facebook:
                    foreach (var path in DependencieAdvertiserPackages)
                        if (path.Contains("audience"))
                            ShowOption(path.Split(Path.DirectorySeparatorChar)[1], path, advertiser.IsInstalled());
                    return;
                case SupportedAdvertisers.IronSource:
                    foreach (var path in DependencieAdvertiserPackages)
                        if (path.Contains("IronSource"))
                            ShowOption(path.Split(Path.DirectorySeparatorChar)[1], path, advertiser.IsInstalled());
                    return;
                case SupportedAdvertisers.AppLovin:
                    foreach (var path in DependencieAdvertiserPackages)
                        if (path.Contains("AppLovin"))
                            ShowOption(path.Split(Path.DirectorySeparatorChar)[1], path, advertiser.IsInstalled());
                    return;
                case SupportedAdvertisers.Unity:
                    InstalledUnityPackage(UnityAd);
                    ShowOption(advertiser.IsInstalled());
                    return;
               
            }
        }
        void ShowOption(string name, string path, bool isInstalled)
        {
            EditorGUILayout.BeginHorizontal(new GUIStyle(EditorStyles.helpBox));
            EditorGUILayout.LabelField(name);

            if (GUILayout.Button(isInstalled ? new GUIContent("Re Install", $"Reinstall {name}") : new GUIContent("Install", $"Install {name}"), new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleCenter, fixedWidth = 80 }))
            {
                AssetDatabase.ImportPackage(path, true);
                installedPackagesFiles.ActivePackage = name;
            }

            if (installedPackagesFiles.HasPackage(name) && GUILayout.Button(new GUIContent("Delete", $"Delete all files from package {name}"), new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleCenter, fixedWidth = 80 }))
            {
                installedPackagesFiles.RemoveInstalled(name);
            }

            EditorGUILayout.EndHorizontal();
        }

        void ShowOption(Adon adon)
        {
            EditorGUILayout.BeginHorizontal(new GUIStyle(EditorStyles.helpBox));


            EditorGUILayout.LabelField(adon.Name);

            if (GUILayout.Button(adon.IsInstalled ? new GUIContent("Re Install", $"Reinstall {adon.Name}") : new GUIContent("Install", $"Install {adon.Name}"), new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleCenter, fixedWidth = 80 }))
            {
                bool found = false;

                foreach (var path in DependenciesAdOnPackages)
                {
                    if (path.Contains(adon.PackageName))
                    {
                        AssetDatabase.ImportPackage(path, true);
                        installedPackagesFiles.ActivePackage = adon.Name;
                        found=true;
                        break;  
                    }
                       
                }
                if (!found)
                    EditorUtility.DisplayDialog("Error Package not found", $"In Dictionary  [{DependenceisAdonsPath}] package [{adon.PackageName}] not found. Please create folder and place dependent packages in it.", "OK");

            }

            if (installedPackagesFiles.HasPackage(adon.Name) && GUILayout.Button(new GUIContent("Delete", $"Delete all files from package {adon.Name}"), new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleCenter, fixedWidth = 80 }))
            { 
                installedPackagesFiles.RemoveInstalled(adon.Name);
            }

            EditorGUILayout.EndHorizontal();
        }

        void ShowOption(bool isInstalled)
        {
            if (string.IsNullOrEmpty(installedPackagesFiles.ActivePackage))
            {
                AddingUnityPackage();
                return;
            }

            EditorGUILayout.BeginHorizontal(new GUIStyle(EditorStyles.helpBox));
            EditorGUILayout.LabelField("com.unity.ads");

            if (GUILayout.Button(isInstalled ? new GUIContent("Re Install", $"Reinstall ") : new GUIContent("Install", $"Install "), new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleCenter, fixedWidth = 80 }))
            {
                PackageManagerInstallRequest = Client.Add(UnityAd);
                installedPackagesFiles.ActivePackage = "com.unity.ads";
            }

            EditorGUILayout.EndHorizontal();
        }
        void InstalledUnityPackage(string name)
        {
            if (PackageManagerInstallRequestList == null)
                PackageManagerInstallRequestList = Client.List();

            if (!PackageManagerInstallRequestList.IsCompleted)
            {
                EditorGUILayout.HelpBox($"Checking for Updates of [{UnityAd}]", MessageType.Warning);
                return;
            }
            if (PackageManagerInstallRequestList.Status == StatusCode.Success)
            {
                var packageInfo = PackageManagerInstallRequestList.Result.FirstOrDefault(p => p.name == name);
                if (packageInfo != null)
                {
                    string installedVersion = packageInfo.versions.latest;
                    string latestVersion = packageInfo.versions.latestCompatible;
                    EditorGUILayout.HelpBox($"Installed version : {installedVersion} || latest Version : {latestVersion}", MessageType.Info);
                }
            }
        }

        private void AddingUnityPackage()
        {
            if (PackageManagerInstallRequest == null)
                return;

            if (!PackageManagerInstallRequest.IsCompleted)
            {
                EditorGUILayout.HelpBox($"Installing unity [{UnityAd}] package.", MessageType.Warning);
                return;
            }

            if (PackageManagerInstallRequest.Status == StatusCode.Success)
            {
                EditorUtility.DisplayDialog("Package successfully installed", $"Package  [{UnityAd}] imported successfully", "OK");
                PackageManagerInstallRequest = null;
                installedPackagesFiles.ActivePackage = string.Empty;
                Extensions.CheckForInstalledPackages();
            }
        }


        private void ImportedItems(string[] obj)
        {
            if (!installedPackagesFiles ||string.IsNullOrEmpty(installedPackagesFiles.ActivePackage))
            {
                return;
            }
            for (int i = 0; i < allAdons.Length; i++)
            {
                allAdons[i].CheckIfInstalled();
            }
            installedPackagesFiles.AddInstalled(installedPackagesFiles.ActivePackage, obj);
           
        }
        private void ImportPackageCompleted(string packageName)
        {
            EditorUtility.DisplayDialog("Package successfully Imported", $"Package  [{packageName}] imported successfully", "OK");
        }
        private void ImportPackageStarted(string packageName)
        {
            
        }
        private void ImportPackageFailed(string packageName, string errorMessage)
        {
            EditorUtility.DisplayDialog("Importing package failled", $"Importing package [{packageName}] failled with error [{errorMessage}]", "OK");
            installedPackagesFiles.ActivePackage = string.Empty;

        }
        private void ImportPackageCanceled(string packageName)
        {
            EditorUtility.DisplayDialog("Importing package Canceled", $"Importing package [{packageName}] is cancelled by user", "OK");
            installedPackagesFiles.ActivePackage = string.Empty;
        }
    }
}
#endif