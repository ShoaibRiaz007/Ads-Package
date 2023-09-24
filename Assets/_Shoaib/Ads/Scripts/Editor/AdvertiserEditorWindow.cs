#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using SH.Ads.Base;
using System;
using SH.Ads.Editor.Base;
using System.IO;
using SH.Ads.Editor;

namespace SH.Ads
{
    public class AdvertiserEditorWindow : Window
    {
        const string JSON_SAVE_PATH = "Assets/_Shoaib/Ads/Json/config.json";
        private AdSettings adSettings;
        private SupportedAdvertisers selectedAdvertiser = SupportedAdvertisers.Admob;
        private AdType selectedAdType = AdType.Banner;
        private string newAdvertiserID = "";
        private bool newAdvertiserIsAndroid = true;
        private bool newAdvertiserIsIOS = false;
        private Dictionary<AdType, bool> adUnitTypeFoldouts = new Dictionary<AdType, bool>();
        private Dictionary<SupportedAdvertisers, bool> advertiserTypeFoldouts = new Dictionary<SupportedAdvertisers, bool>();
        private Vector2 advertiserScrollPos;


        static GUIStyle ButtonStyle;

        [MenuItem("SH/Ad Manager")]
        public static void ShowWindow()
        {
            GetWindow<AdvertiserEditorWindow>("Ad Manager");
            Extensions.CheckForInstalledPackages();
            
        }

        private void OnEnable()
        {
            adSettings = Resources.Load<AdSettings>("AdSetting");
            if (adSettings == null)
            {
                adSettings = CreateInstance<AdSettings>();

                string resourcesFolderPath = "Assets/_Shoaib/Ads/Resources";
                if (!Directory.Exists(resourcesFolderPath))
                {
                    Directory.CreateDirectory(resourcesFolderPath);
                }
                string assetPath = "Assets/_Shoaib/Ads/Resources/AdSetting.asset";

                AssetDatabase.CreateAsset(adSettings, assetPath);
                AssetDatabase.SaveAssets();
                Selection.activeObject = adSettings;
            }
        }
        private void OnDisable()
        {
            EditorUtility.SetDirty(adSettings);
        }

        protected override void OnGUI()
        {
            base.OnGUI();
            
            if(ButtonStyle==null)
                ButtonStyle = new GUIStyle(EditorStyles.objectField) { fixedHeight = 40, padding = new RectOffset() { top = 10 } };

            Header();
            EditorGUILayout.LabelField("Manage Advertisers", EditorStyles.boldLabel);
            
            advertiserScrollPos = EditorGUILayout.BeginScrollView(advertiserScrollPos, EditorStyles.helpBox);
            foreach (var advertiser in adSettings.advertisers)
            {
                DisplayAdvertiser(advertiser);
            }
            EditorGUILayout.EndScrollView();
            Footer();
        }
        void Footer()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            if (GUILayout.Button(new GUIContent("To Json", "Convert to json for remote config or custom API call")))
            {
               string json = JsonUtility.ToJson(adSettings,true);
               File.WriteAllText(JSON_SAVE_PATH, json);
                AssetDatabase.Refresh();
                Selection.activeObject= AssetDatabase.LoadAssetAtPath(JSON_SAVE_PATH, typeof(TextAsset));
            }
            if (GUILayout.Button(new GUIContent("Select Ad Setting", "Ad setting scriptable object")))
            {
                Selection.activeObject = adSettings;
            }
            EditorGUILayout.EndHorizontal();
        }
        void Header()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.HelpBox("You can add multiple advertisers. When you add an advertiser, it may take some time as it requires recompilation of the code. You can edit advertiser IDs even after they have been added.", MessageType.Info);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Add New Advertiser", EditorStyles.boldLabel);
            FilteredAdvertiserDropdown();
            newAdvertiserID = EditorGUILayout.TextField(new GUIContent("Advertiser ID:", "Enter a unique ID for the advertiser"), newAdvertiserID);
            EditorGUILayout.BeginHorizontal();
            newAdvertiserIsAndroid = EditorGUILayout.Toggle(new GUIContent("Is Android:", "Check if this advertiser is for Android"), newAdvertiserIsAndroid);
            newAdvertiserIsIOS = EditorGUILayout.Toggle(new GUIContent("Is iOS:", "Check if this advertiser is for iOS"), newAdvertiserIsIOS);
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button(new GUIContent("Add Advtiser", "Add selectd Advertiser to active list")))
            {
                OnAddAdvertiserClick();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        private void OnAddAdvertiserClick()
        {
            if (!adSettings.advertisers.Any(advertiser => advertiser.advertiser == selectedAdvertiser))
            {
                if (selectedAdvertiser.IsInstalled())
                {
                    selectedAdvertiser.AddToRegistry();
                    Advertiser newAdvertiser = new Advertiser
                    {
                        advertiser = selectedAdvertiser,
                        ID = newAdvertiserID,
                        IsAndroid = newAdvertiserIsAndroid,
                        IsIOS = newAdvertiserIsIOS,
                        Ads = new List<AD>()
                    };
                    adSettings.advertisers.Add(newAdvertiser);
                }
                else
                    PackageInstallerWindow.ShowWindow();
            }
            else
                EditorUtility.DisplayDialog("Warning", $"Advertiser [{selectedAdType}] already exist in active list.", "OK");
        }
        private void FilteredAdvertiserDropdown()
        {
            List<SupportedAdvertisers> availableAdvertisers = Enum.GetValues(typeof(SupportedAdvertisers))
                .Cast<SupportedAdvertisers>()
                .Where(advertiser => !adSettings.advertisers.Any(a => a.advertiser == advertiser))
                .ToList();

            int selectedIndex = availableAdvertisers.IndexOf(selectedAdvertiser);
            selectedIndex = EditorGUILayout.Popup(new GUIContent("Advertiser:", "Select an advertiser to add"), selectedIndex, availableAdvertisers.Select(a => a.ToString()).ToArray());

            if (selectedIndex >= 0 && selectedIndex < availableAdvertisers.Count)
            {
                selectedAdvertiser = availableAdvertisers[selectedIndex];
            }
        }
        private void FilteredAdUnitDropdown(Advertiser advertiser)
        {
            List<AdType> availableAdTypes = Enum.GetValues(typeof(AdType))
                .Cast<AdType>()
                .Where(adType =>
                    advertiser.advertiser.SupportsAd(adType) &&
                    !advertiser.Ads.Any(ad => ad.adType == adType))
                .ToList();
            int selectedIndex = availableAdTypes.IndexOf(selectedAdType);
            selectedIndex = EditorGUILayout.Popup(new GUIContent("", "Select Ad unit type to add"), selectedIndex, availableAdTypes.Select(adType => adType.ToString()).ToArray());

            if (selectedIndex >= 0 && selectedIndex < availableAdTypes.Count)
            {
                selectedAdType = availableAdTypes[selectedIndex];
            }
        }

        private void DisplayAdvertiser(Advertiser advertiser)
        {
            EditorGUILayout.Space();

            if (!advertiserTypeFoldouts.ContainsKey(advertiser.advertiser))
                advertiserTypeFoldouts[advertiser.advertiser] = false;


            EditorGUILayout.BeginHorizontal(ButtonStyle);
            advertiserTypeFoldouts[advertiser.advertiser] = EditorGUILayout.Foldout(advertiserTypeFoldouts[advertiser.advertiser], advertiser.advertiser.ToString(), true, new GUIStyle(EditorStyles.foldout) { fontStyle = FontStyle.Bold});

            EditorGUILayout.TextField(new GUIContent("", "Current version of advertiser"), advertiser.advertiser.InstalledVersion(), EditorStyles.boldLabel);
            if (GUILayout.Button(new GUIContent("Update", $"Check of {advertiser.advertiser} update"), new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleCenter, fixedWidth = 80 }))
            {
                advertiser.advertiser.CheckUpdate();
            }
            if (GUILayout.Button(new GUIContent("-", "Remove this Advertiser"), new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleCenter, fixedWidth = 80 }))
            {
                adSettings.advertisers.Remove(advertiser);
                advertiser.advertiser.RemovefromRegistry();
            }
            EditorGUILayout.EndHorizontal();
            if (!advertiserTypeFoldouts[advertiser.advertiser])
                return;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            advertiser.ID=EditorGUILayout.TextField(new GUIContent("Advertiser ID:", "Unique ID for the advertiser"), advertiser.ID.ToString());
            advertiser.order=EditorGUILayout.IntField(new GUIContent("Order :", "Priorty of this advertiser lower is better"), advertiser.order);
            adSettings.advertisers = adSettings.advertisers.OrderBy(a => a.order).ToList();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Ad Units", EditorStyles.boldLabel);
            FilteredAdUnitDropdown(advertiser);

            if (GUILayout.Button(new GUIContent("+", "Add selected Ad Unit"), new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleCenter, fixedWidth = 80 }))
            {
                if (!advertiser.Ads.Any(ad => ad.adType == selectedAdType))
                    advertiser.Ads.Add(new AD() { adType = selectedAdType, adIds = new List<string>() });
                else
                    EditorUtility.DisplayDialog("Warning", $"Ad unit [{selectedAdType}] already exist in current Advertiser.", "OK");
            }
            EditorGUILayout.EndHorizontal();

            foreach (var ad in advertiser.Ads)
            {
                DisplayAdUnit(ad, advertiser);
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(10);
        }

        private void DisplayAdUnit(AD ad, Advertiser advertiser)
        {
            if (!adUnitTypeFoldouts.ContainsKey(ad.adType))
            {
                adUnitTypeFoldouts[ad.adType] = true;
            }
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();
            adUnitTypeFoldouts[ad.adType] = EditorGUILayout.Foldout(adUnitTypeFoldouts[ad.adType], ad.adType.ToString(), true, new GUIStyle(EditorStyles.foldout) { fontStyle= FontStyle.Bold});
            if((ad.adType != AdType.Banner && ad.adType != AdType.BigBanner))
            {
                ad.loadAtStart = EditorGUILayout.ToggleLeft(new GUIContent("Load Ad At Start", "Load ad after intialization."), ad.loadAtStart, GUILayout.Width(120));
                ad.loadAfterClose = EditorGUILayout.ToggleLeft(new GUIContent("Load AD On Close", "Load ad after showing ad is closed."), ad.loadAfterClose, GUILayout.Width(130));
            }else if (adSettings.advertisers.Select(a=>a.Ads).All(a => a.Where(a => (a.adType == AdType.Banner || a.adType == AdType.BigBanner) && a.loadAtStart).Count()==0))
                ad.loadAtStart = EditorGUILayout.ToggleLeft(new GUIContent("Show banner at start", "Load ad and show after intialization."), ad.loadAtStart, GUILayout.Width(150));
            else if(ad.loadAtStart)
                ad.loadAtStart = EditorGUILayout.ToggleLeft(new GUIContent("Show banner at start", "Load ad and show after intialization."), ad.loadAtStart, GUILayout.Width(150));
            if (GUILayout.Button(new GUIContent("+", "Add new Ad ID"), GUILayout.Width(80)))
            {
                ad.adIds.Add(string.Empty);
            }
            if (GUILayout.Button(new GUIContent("-", "Remove Ad Unit"), GUILayout.Width(80)))
            {
                advertiser.Ads.Remove(ad);
            }
            EditorGUILayout.EndHorizontal();
            if (adUnitTypeFoldouts[ad.adType])
            {
                DisplayAdIDsField(ad);
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }
        private void DisplayAdIDsField(AD ad)
        {
            GUILayout.BeginVertical();
            for (int i = 0; i < ad.adIds.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                ad.adIds[i] = EditorGUILayout.TextField(new GUIContent($"ID {i + 1}:", "Enter the ID for this ad unit"), ad.adIds[i]);
                if (GUILayout.Button(new GUIContent("-", "Remove Ad ID"), GUILayout.Width(80)))
                {
                    ad.adIds.RemoveAt(i);
                }
                EditorGUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }

        
    }
}
#endif