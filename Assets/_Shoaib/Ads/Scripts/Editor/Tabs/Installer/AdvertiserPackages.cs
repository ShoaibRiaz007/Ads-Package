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
    public class AdvertiserPackages : IInstaller
    {
        const string DependenceisPath = "Assets/_Shoaib/Ads/Dependencies";
        const string UnityAd = "com.unity.ads";
        List<string> DependencieAdvertiserPackages = new List<string>();
        ListRequest PackageManagerInstallRequestList;
        AddRequest PackageManagerInstallRequest;
        InstalledPackages installedPackagesFiles = null;

        public override GUIContent Title => new GUIContent("Advertisers Packages", Texture(), "Tab To list down Advertisers Packages");
        protected override string IconBase64 => "iVBORw0KGgoAAAANSUhEUgAAABkAAAAZCAYAAADE6YVjAAAE80lEQVRIDYWWfUyVVRjA38u9ShfKiRMycKJQTFy7EI665TJhEQMk8I8IcQOaMWCLYWMWuSWwZqSBlQjB2ICmwXBBKSQKDAomwaRBDEgjBsXHGB8hH4GkQL+HeNlNr5ez/XbOeb7OOc/5eF9leXlZWY/Z2Vn9gQMHXlYU5RJ0QTsUZ2VlvTI5OfnY4uKixRgWlTL4+Pj4EydOnDhO0KawsLCU8vJyP3ixtLQ0FtllSJyZmbG2NFGLg8zPz2uSk5MPEugn8BoYGNCowe7evWuFzAhXz54966XKzdVmB6mqqtK7ubmFuLi4XCHIFBydm5tbG0ACoT+E/Ds4A030A/AzuyLl3r17Vn19ffbgQdBwHLKhHr4/derUSeprnZ2dPhIYm22wJTExMRrbX+rq6vYXFxcHYNMP10H8stCFSTzYKvEVcnsMxQUohi8aGhoiwQOsybUtsi9zcnJC7t+/T1NJBrGrbGpqMsrADCIruoi9Hrzi4+Pfon9u1e7CanwlD0FNc3OzYXR0VG+aUwbRopNNT2cPdB0dHc/STqM2iF1vb68uICAgA1mk6nfnzh2FODbE80BeA3nK1NTUZj8/P5ldOm09gXWwiZS4IHsXbkAf7Ee+cWFhQcP+KBMTE9bI5FD8BqnYu4qf+EscZOkSV+KvbDy52xUVFfU1ikyQmUv6KoxG4+ekIyQmJiaK/o/wcW5ubkRsbGwE7Y/gCvv2Pnaf0a4A8RP/TOKVSFxZ4drpys7O9kf5N/wJMvO0iIgIZzEaHBzUsPkGZG/DJ3CawDHI9pAerdghS4V+GIARZO+RTqu1QVpbWzdwBAtQnrl9+/ZW6jegysHB4S+WfBmCSJOWfdHABmF6elqHPFD02E1iL6frTfztqWU/2ry9vV9nEhqFU6OprKwMRFiVmppqKyOrpKWl7YCk4ODgWvSyN3InJHUrdwN5HfrjsFP1kZpnRpOQkCBH+wfwVXh7ZJNkL2qqq6tDa2trnUxvtjh1dXXZIH8BmyIgyUo1fWN3d7eNaXBpt7e324ptYWFhInY3oURWomUlu+nIfZEb3BodHZ3L7F49f/68Tg2SmZmpQyerkRdggP4+VSc19gY4Sfoa0P8KpyGQiexaeSrorBWNRvONl5fXZs66GxsuN/AiyBEPhhhog1twFA7D83BkO8Xd3f06m92v0+kOszdPI/+vmM5G2pQibm4gq7MpKCh4LSUl5VNkV+FnkKBFrEICfAg16M9hdwj7LeKfkZHhzyH63TSupMBsCQoKmkNRzZ7VhoaGbqP9JKub8aeEh4cv+/r6SjrynJ2dJ+zs7BbNBlkVPnIQ1YkASzBMf5jiqso9PT3/oT2q9i3V8k14ZGF/4vLy8hzGxsac4alHGq6jsDgIvt/GxcUtcdki4VhLS4tmnXhm1eul6x28doC8pq2Ojo4rJ8NsJEtC01OwerpKsC+HvbAJ5FL1QDsfo2eoi4aGhlxVv6WlJbnhViMjI3Kp5f2r4nTdVPVSP7QSHsoPMDxSVlZ2mi/fNG25gLFg1djYuOTk5ESTUXt65IO2s6KiYndSUtJLtN15nRcMBsM1rVZ7SWzU8tBlVBW8sPb8qTzn4+NjRLYHdPn5+X/w7O+lvkG9HZkMNFxfX9+m1+s7uI+3mIS85P8vpst6sC3/U9yTjeDAf9c+PPNBPlJf0T+I3AkelwfxQV/T/r8GA7mzH/GvAgAAAABJRU5ErkJggg==";
        public override void OnEnable(AdSettings settings)
        {
            Extensions.CheckForInstalledPackages();
            ListFilesInFolder();
            EditorUtility.SetDirty(settings);
            installedPackagesFiles =InstalledPackages.Load();
        }

        public override void OnGUI()
        {
            Header();
            if (string.IsNullOrEmpty(installedPackagesFiles.ActivePackage))
            {
               EditorGUILayout.Space(30);
                foreach (var t in Enum.GetValues(typeof(SupportedAdvertisers)).Cast<SupportedAdvertisers>())
                    ShowAdvertiser(t);
            }
            else
            {
                EditorGUILayout.HelpBox("Importing package is in progress. Please wait for it complete. \n Some times it get stuck when you close it with X on rigt upper corner.", MessageType.Warning);
                if (GUILayout.Button(new GUIContent("Click Here if stuck"), new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleCenter, fixedHeight = 50 }))
                {
                    installedPackagesFiles.ActivePackage= string.Empty;
                }
            }
        }
        void ListFilesInFolder()
        {
            if (Directory.Exists(DependenceisPath))
            {
                DependencieAdvertiserPackages = new List<string>();
                DependencieAdvertiserPackages.AddRange(Directory.GetFiles(DependenceisPath));
                DependencieAdvertiserPackages = DependencieAdvertiserPackages.Where(a => !a.Contains(".meta")).ToList();
            }
            else
                EditorUtility.DisplayDialog("Error", $"Dictionary  [{DependenceisPath}] not found. Please create folder and place dependent packages in it.", "OK");

        }

        void Header()
        {
            EditorGUILayout.HelpBox("Bellow are all the supported and tested packages. You can install and activate any one of those", MessageType.Info);
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
    }
}
#endif