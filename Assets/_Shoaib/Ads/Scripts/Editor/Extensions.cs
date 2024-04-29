#if UNITY_EDITOR
using UnityEditor;
using System;
using System.Reflection;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using SH.Ads.Editor;
using SH.Ads;

namespace SH
{
    public static class FileHelper
    {
        public static void CreateFolderHierarchy()
        {
            string[] Paths = new string[]
            {
                "Assets/_Shoaib/Ads/Json",
                 "Assets/_Shoaib/Ads/Resources",
                "Assets/_Shoaib/Ads/Data",
                "Assets/_Shoaib/Ads/Scripts/Editor/InstallHistory/Data"
            };
            foreach(var path in Paths)
            {
                string[] folders = path.Split('/');

                string currentPath = "";

                foreach (string folder in folders)
                {
                    currentPath = Path.Combine(currentPath, folder);

                    if (!Directory.Exists(currentPath))
                    {
                        Debug.Log("Creating folder: " + currentPath);
                        Directory.CreateDirectory(currentPath);
                        AssetDatabase.Refresh();
                    }
                }
            }
        }
    }


    public static class Extensions
    {
        static Dictionary<Ads.SupportedAdvertisers, string> Versions = new Dictionary<Ads.SupportedAdvertisers, string>();
        static Dictionary<Ads.SupportedAdvertisers, bool> Installed = new Dictionary<Ads.SupportedAdvertisers, bool>();
        static Dictionary<string, bool> SupportedAdsTypes = new Dictionary<string, bool>();
        public static bool SupportsAd(this Ads.SupportedAdvertisers advertiser, Ads.AdType adtype )
        {
            string tem = $"SH.Ads.{advertiser}.{adtype.ToString().Replace("Big", string.Empty)}";
            if (!SupportedAdsTypes.ContainsKey(tem))
                CheckforSuportedTypes();

            return SupportedAdsTypes[tem];
        }
        public static bool IsInstalled(this Ads.SupportedAdvertisers advertiser)
        {
            if (!Installed.ContainsKey(advertiser))
                CheckForInstalledPackages();
            return Installed[advertiser];
        }
        static void CheckforSuportedTypes()
        {
            SupportedAdsTypes = new Dictionary<string, bool>();
            foreach(var advertiser in Enum.GetValues(typeof(Ads.SupportedAdvertisers)))
            {
                foreach(var adType in Enum.GetValues(typeof(Ads.AdType)))
                {
                    var tem = $"SH.Ads.{advertiser}.{adType.ToString().Replace("Big", string.Empty)}";
                    if(!SupportedAdsTypes.ContainsKey(tem))
                        SupportedAdsTypes.Add(tem, Assembly.Load("Assembly-CSharp")?.GetType(tem)!=null);
                }
            }
        }
        public static void CheckForInstalledPackages()
        {
            Installed = new Dictionary<Ads.SupportedAdvertisers, bool>();
            foreach(var advertiser in Enum.GetValues(typeof(Ads.SupportedAdvertisers)))
            {
                bool isInsalled = false;
                switch (advertiser)
                {
                    case Ads.SupportedAdvertisers.AdColony:
                        isInsalled = Assembly.Load("Assembly-CSharp")?.GetType("AdColony.Ads") != null;
                        break;
                    case Ads.SupportedAdvertisers.Admob:
                        try
                        {
                            isInsalled = Assembly.Load("GoogleMobileAds")?.GetType("GoogleMobileAds.Api.MobileAds") != null;
                        }
                        catch (Exception)
                        {
                            isInsalled = false;
                        }
                        break;
                    case Ads.SupportedAdvertisers.Unity:
                        isInsalled = IsPackageInstalled("com.unity.ads");
                        break;
                    case Ads.SupportedAdvertisers.Facebook:
                        isInsalled = Assembly.Load("Assembly-CSharp")?.GetType("AudienceNetwork.AudienceNetworkAds") != null;
                        break;
                    case Ads.SupportedAdvertisers.IronSource:
                        isInsalled = Assembly.Load("Assembly-CSharp")?.GetType("IronSource") != null;
                        break;
                    case Ads.SupportedAdvertisers.AppLovin:
                        try
                        {
                            isInsalled = Assembly.Load("MaxSdk.Scripts")?.GetType("MaxSdk") != null;
                        }
                        catch
                        {
                            isInsalled= false;
                        }
                        
                        break;
                    default:
                        Debug.LogError("Not Implemented yet");
                        isInsalled = false;
                        break;
                }

                if (Installed.ContainsKey((Ads.SupportedAdvertisers)advertiser))
                    Installed.Add((Ads.SupportedAdvertisers)advertiser, isInsalled);
                Installed[(Ads.SupportedAdvertisers)advertiser] = isInsalled;
            }

            
        }
        public static void UpdateAdmobSettings(this Ads.Base.Advertiser advertiser)
        {
            var settingType = Assembly.Load("GoogleMobileAds.Editor")?.GetType("GoogleMobileAds.Editor.GoogleMobileAdsSettings");
            if (settingType != null)
            {
                var settings = Resources.Load("GoogleMobileAdsSettings");
                if(!settings)
                {
                    EditorUtility.DisplayDialog("No google settings found. ","Can you add google settings and try again. You can do that by selecting \n Assets->Google Mobile Ads->Settings...", "OK");
                    return;
                }
                PropertyInfo propertyInfo;
                if (advertiser.IsAndroid)
                {
                    propertyInfo = settingType.GetProperty("GoogleMobileAdsAndroidAppId");
                    if (propertyInfo != null)
                    {
                        propertyInfo.SetValue(settings, advertiser.ID, null);
                    }
                }
                else if(advertiser.IsIOS)
                {
                    propertyInfo = settingType.GetProperty("GoogleMobileAdsIOSAppId");
                    if (propertyInfo != null)
                    {
                        propertyInfo.SetValue(settings, advertiser.ID, null);
                    }
                }
                propertyInfo = settingType.GetProperty("DelayAppMeasurementInit");
                if (propertyInfo != null)
                {
                    propertyInfo.SetValue(settings, true, null);
                }
                propertyInfo = settingType.GetProperty("OptimizeInitialization");
                if (propertyInfo != null)
                {
                    propertyInfo.SetValue(settings, true, null);
                }
                propertyInfo = settingType.GetProperty("OptimizeAdLoading");
                if (propertyInfo != null)
                {
                    propertyInfo.SetValue(settings, true, null);
                }

                EditorUtility.SetDirty(settings);

            }
        }

        public static string InstalledVersion(this Ads.SupportedAdvertisers advertiser)
        {
            if (Versions.ContainsKey(advertiser))
            {
                return Versions[advertiser];
            }

            string version = string.Empty;

            switch (advertiser)
            {
                case Ads.SupportedAdvertisers.Admob:
                    string changelogPath = "Assets/GoogleMobileAds/CHANGELOG.md";
                    if (File.Exists(changelogPath))
                    {
                        string[] lines = File.ReadAllLines(changelogPath);
                        bool foundVersion = false;

                        foreach (string line in lines)
                        {
                            if (line.Contains("********"))
                            {
                                foundVersion = true;
                            }
                            else if (foundVersion)
                            {
                                version = line.Trim();
                                Debug.Log("AdMob Version: " + version);
                                Versions.Add(advertiser, version);
                                break;
                            }
                        }

                        if (!foundVersion)
                        {
                            Debug.LogError("Marker (********) not found in the file.");
                        }
                    }
                    else
                    {
                        Debug.LogError("File does not exist: " + changelogPath);
                    }
                    break;

                case Ads.SupportedAdvertisers.AdColony:
                    var assembly = Assembly.Load("Assembly-CSharp");
                    if (assembly != null)
                    {
                        Type type = assembly.GetType("AdColony.Constants");
                        if (type != null)
                        {
                            var propertyInfo = type.GetField("AdapterVersion", BindingFlags.Public | BindingFlags.Static);
                            if (propertyInfo != null)
                            {
                                version = "Version " + propertyInfo.GetValue(null);
                                Versions.Add(advertiser, version);
                            }
                        }
                    }
                    break;
                case Ads.SupportedAdvertisers.IronSource:
                    assembly = Assembly.Load("Assembly-CSharp");
                    if (assembly != null)
                    {
                        Type type = assembly.GetType("IronSource");
                        if (type != null)
                        {
                            var propertyInfo = type.GetField("UNITY_PLUGIN_VERSION", BindingFlags.Public | BindingFlags.Static);
                            if (propertyInfo != null)
                            {
                                version = "Version " + propertyInfo.GetValue(null);
                                Versions.Add(advertiser, version);
                            }
                        }
                    }
                    break;

                case Ads.SupportedAdvertisers.Unity:
                    version = "Version " + PackageCurrentVersion("com.unity.ads");
                    Versions.Add(advertiser, version);
                    break;

                case Ads.SupportedAdvertisers.Facebook:
                    assembly = Assembly.Load("Assembly-CSharp");
                    if (assembly != null)
                    {
                        Type type = assembly.GetType("AudienceNetwork.SdkVersion");
                        if (type != null)
                        {
                            var propertyInfo = type.GetProperty("Build", BindingFlags.Public | BindingFlags.Static);
                            if (propertyInfo != null)
                            {
                                version = "Version " + propertyInfo.GetValue(null);
                                Debug.Log(version);
                                Versions.Add(advertiser, version);
                            }
                        }
                    }
                    break;
                case Ads.SupportedAdvertisers.AppLovin:
                    assembly = Assembly.Load("MaxSdk.Scripts");
                    if (assembly != null)
                    {
                        Type type = assembly.GetType("MaxSdk");
                        if (type != null)
                        {
                            var propertyInfo = type.GetProperty("Version", BindingFlags.Public | BindingFlags.Static);
                            if (propertyInfo != null)
                            {
                                version = "Version " + propertyInfo.GetValue(null);
                                Debug.Log(version);
                                Versions.Add(advertiser, version);
                            }
                        }
                    }
                    break;

                default:
                    return "N.A";
            }

            return version;
        }
        public static void CheckUpdate(this Ads.SupportedAdvertisers advertiser)
        {
            switch (advertiser)
            {
                case Ads.SupportedAdvertisers.Admob:
                    Application.OpenURL("https://github.com/googleads/googleads-mobile-unity");
                    return;
                case Ads.SupportedAdvertisers.AdColony:
                    Application.OpenURL("https://github.com/AdColony/AdColony-Unity-Plugin");
                    return;
                case Ads.SupportedAdvertisers.Unity:
                    AdvertiserEditorWindow.ShowPanel<InstallPackage>();
                    return;
                case Ads.SupportedAdvertisers.Facebook:
                    Application.OpenURL("https://developers.facebook.com/docs/audience-network/setting-up/platform-steup/unity/add-sdk/");
                    return;
                case Ads.SupportedAdvertisers.IronSource:
                    Application.OpenURL("https://github.com/ironsource-mobile/Unity-sdk");
                    return;
            }
        }
        public static void RemovefromRegistry(this Ads.SupportedAdvertisers advertiser)
        {
            RemoveSymbol(advertiser.ToString());
        }
        public static void AddToRegistry(this Ads.SupportedAdvertisers advertiser)
        {
            AddSymbol(advertiser.ToString());
        }

        public static void AddToRegistry(string PackageID)
        {
            AddSymbol(PackageID);
        }
        public static void RemovefromRegistry(string PackageID)
        {
            RemoveSymbol(PackageID);
        }
        internal static void AddSymbol(string symbolToAdd)
        {
            BuildTargetGroup targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);

            if (!defines.Contains(symbolToAdd))
            {
                defines += ";" + symbolToAdd;
                PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, defines);
                EditorUtility.DisplayDialog("Symbol Added", $"The '{symbolToAdd}' symbol has been added.", "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("Symbol Already Present", $"The '{symbolToAdd}' symbol is already defined.", "OK");
            }
        }
        internal static void RemoveSymbol(string symbolToRemove)
        {
            BuildTargetGroup targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);

            if (defines.Contains(symbolToRemove))
            {
                defines = defines.Replace(symbolToRemove, "");
                PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, defines);
                EditorUtility.DisplayDialog("Symbol Removed", $"The '{symbolToRemove}' symbol has been removed.", "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("Symbol Not Found", $"The '{symbolToRemove}' symbol is not defined.", "OK");
            }
        }
        static bool IsPackageInstalled(string packageId)
        {
            string jsonText = File.ReadAllText("Packages/manifest.json");
            return jsonText.Contains(packageId);
        }
        static string PackageCurrentVersion(string packageId)
        {
            string jsonText = File.ReadAllText("Packages/manifest.json");
            int startIndex = jsonText.IndexOf(packageId);
            startIndex += packageId.Length+4;
            int endIndex = jsonText.IndexOf('\n', startIndex)-2;
            string subString = jsonText.Substring(startIndex, endIndex - startIndex);
            return subString;
        }
    }
}
#endif