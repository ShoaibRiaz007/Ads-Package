#if UNITY_EDITOR
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
    public class PackageInstallerWindow : Window
    {
        const string DependenceisPath = "Assets/_Shoaib/Ads/Dependencies";
        const string UnityAd = "com.unity.ads";
        static   bool ImportingInProgress=false,InstallingUnityPackage=false;
        static List<string> files = new List<string>();
        static ListRequest InstalledPackages;
        static AddRequest InstallPackage;
        
        [MenuItem("SH/Package Manager")]
        public static void ShowWindow()
        {
            GetWindow<PackageInstallerWindow>("Install Manager");
        }
        private void OnFocus()
        {
            Extensions.CheckForInstalledPackages();
            ListFilesInFolder(DependenceisPath);
            ImportingInProgress = false;
        }
        static void ListFilesInFolder(string path)
        {
            if (Directory.Exists(path))
            {
                files = new List<string>();
                files.AddRange(Directory.GetFiles(path));
                files=files.Where(a=>!a.Contains(".meta")).ToList();
            }
            else
                EditorUtility.DisplayDialog("Error", $"Dictionary  [{DependenceisPath}] not found. Please create folder and place dependent packages in it.", "OK");
            
        }
        Vector2 scrollPos;
        protected override void OnGUI()
        {
            base.OnGUI();
            Header();
            scrollPos=EditorGUILayout.BeginScrollView(scrollPos, new GUIStyle(EditorStyles.helpBox));
            if(!ImportingInProgress)
                foreach (var t in Enum.GetValues(typeof(SupportedAdvertisers)).Cast<SupportedAdvertisers>())
                    ShowAdvertiser(t);
            else
                EditorGUILayout.HelpBox("Importing package is in progress. Please wait for it complete.", MessageType.Warning);

            EditorGUILayout.EndScrollView();
        }

        void Header()
        {
            EditorGUILayout.HelpBox("This window is used to install the dependencies of Advertisment package. Bellow are all the supported and tested packages.", MessageType.Info);
            EditorGUILayout.Space();
        }
        void ShowAdvertiser(SupportedAdvertisers advertiser)
        {
           
            EditorGUILayout.LabelField(advertiser.ToString(),EditorStyles.boldLabel);
            switch (advertiser)
            {
                case SupportedAdvertisers.Admob:
                    foreach (var path in files)
                        if (path.Contains("GoogleMobileAds"))
                            ShowOption(path.Split('\\')[1], path,advertiser.IsInstalled());
                    return;
                case SupportedAdvertisers.AdColony:
                    foreach (var path in files)
                        if (path.Contains("AdColony"))
                            ShowOption(path.Split('\\')[1], path, advertiser.IsInstalled());
                    return;
                case SupportedAdvertisers.Facebook:
                    foreach (var path in files)
                        if (path.Contains("audience"))
                            ShowOption(path.Split('\\')[1], path, advertiser.IsInstalled());
                    return;
                case SupportedAdvertisers.IronSource:
                    foreach (var path in files)
                        if (path.Contains("IronSource"))
                            ShowOption(path.Split('\\')[1], path, advertiser.IsInstalled());
                    return;
                case SupportedAdvertisers.Unity:
                    InstalledUnityPackage(UnityAd);
                    ShowOption(advertiser.IsInstalled());
                    return;
            }
        }
        void ShowOption(string name, string path,bool isInstalled)
        {
            EditorGUILayout.BeginHorizontal(new GUIStyle(EditorStyles.helpBox));
            EditorGUILayout.LabelField(name);

            if (GUILayout.Button(isInstalled? new GUIContent("Re Install", $"Reinstall {name}") : new GUIContent("Install", $"Install {name}"), new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleCenter, fixedWidth = 80 }))
            {
                AssetDatabase.importPackageCompleted += ImportPackageCompleted;
                AssetDatabase.importPackageCancelled += ImportPackageCanceled;
                AssetDatabase.importPackageStarted += ImportPackageStarted;
                AssetDatabase.importPackageFailed += ImportPackageFailed;
                AssetDatabase.ImportPackage(path,true);
            }
            
            EditorGUILayout.EndHorizontal();
        }

        void ShowOption(bool isInstalled)
        {
            if (InstallingUnityPackage)
            {
                AddingUnityPackage();
                return;
            }

            EditorGUILayout.BeginHorizontal(new GUIStyle(EditorStyles.helpBox));
            EditorGUILayout.LabelField("com.unity.ads");

            if (GUILayout.Button(isInstalled ? new GUIContent("Re Install", $"Reinstall {name}") : new GUIContent("Install", $"Install {name}"), new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleCenter, fixedWidth = 80 }))
            {
                InstallPackage = Client.Add(UnityAd);
                InstallingUnityPackage = true;
            }
            EditorGUILayout.EndHorizontal();
        }
        void InstalledUnityPackage(string name)
        {
            if(InstalledPackages==null)
                InstalledPackages = Client.List();

            if(!InstalledPackages.IsCompleted)
            {
                EditorGUILayout.HelpBox($"Checking for Updates of [{UnityAd}]", MessageType.Warning);
                return;
            }
            if (InstalledPackages.Status == StatusCode.Success)
            {
                var packageInfo = InstalledPackages.Result.FirstOrDefault(p => p.name == name);
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
            if (InstallPackage == null)
                return;

            if (!InstallPackage.IsCompleted)
            {
                EditorGUILayout.HelpBox($"Installing unity [{UnityAd}] package.", MessageType.Warning);
                return;
            }

            if(InstallPackage.Status == StatusCode.Success)
            {
                EditorUtility.DisplayDialog("Package successfully installed", $"Package  [{UnityAd}] imported successfully", "OK");
                InstallPackage=null;
                InstallingUnityPackage = false;
                Extensions.CheckForInstalledPackages();
            }
        }

        private void ImportPackageCompleted(string packageName)
        {
            ImportingInProgress = false;
            EditorUtility.DisplayDialog("Package successfully Imported", $"Package  [{packageName}] imported successfully", "OK");
            AssetDatabase.importPackageCompleted -= ImportPackageCompleted;
            AssetDatabase.importPackageCancelled -= ImportPackageCanceled;
            AssetDatabase.importPackageStarted -= ImportPackageStarted;
            AssetDatabase.importPackageFailed -= ImportPackageFailed;
        }
        private void ImportPackageCanceled(string packageName)
        {
            ImportingInProgress = false;
            EditorUtility.DisplayDialog("Importing package Canceled", $"Importing package [{packageName}] is cancelled by user", "OK");
            AssetDatabase.importPackageCompleted -= ImportPackageCompleted;
            AssetDatabase.importPackageCancelled -= ImportPackageCanceled;
            AssetDatabase.importPackageStarted -= ImportPackageStarted;
            AssetDatabase.importPackageFailed -= ImportPackageFailed;
        }
        private void ImportPackageStarted(string packageName)
        {
            ImportingInProgress = true;
        }
        private void ImportPackageFailed(string packageName, string errorMessage)
        {
            ImportingInProgress = false;
            EditorUtility.DisplayDialog("Importing package failled", $"Importing package [{packageName}] failled with error [{errorMessage}]", "OK");
            AssetDatabase.importPackageCompleted -= ImportPackageCompleted;
            AssetDatabase.importPackageCancelled -= ImportPackageCanceled;
            AssetDatabase.importPackageStarted -= ImportPackageStarted;
            AssetDatabase.importPackageFailed -= ImportPackageFailed;
            
        }
    }
}
#endif