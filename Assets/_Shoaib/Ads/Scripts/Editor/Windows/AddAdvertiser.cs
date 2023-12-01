#if UNITY_EDITOR
using SH.Ads.Base;
using SH.Ads.Editor.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace SH.Ads.Editor
{
    public class AddAdvertiser : IWindow
    {
        public override string Name => "Add Advertiser";

        public override string ToolTip => "Panel to add new advertiser to active list";

        static AdSettings AdSetting;
        static SupportedAdvertisers selectedAdvertiser = SupportedAdvertisers.Admob;
        bool newAdvertiserIsAndroid = true;
        bool newAdvertiserIsIOS = false;
        string newAdvertiserID = string.Empty;

        public override void OnEnable(AdSettings settings)
        {
            AdSetting = settings;
            EditorUtility.SetDirty(settings);
        }

        public override void OnGUI()
        {
            Header();
            Content();
        }
        void Header()
        {
            EditorGUILayout.HelpBox("This window is used to used to add Advertisers in the project. ", MessageType.Info);
            EditorGUILayout.Space();
        }

        void Content()
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
            if (!AdSetting.CurrentPipline.Advertisers.Any(advertiser => advertiser.advertiser == selectedAdvertiser))
            {
                if (selectedAdvertiser.IsInstalled())
                {

                    Ads.Base.Advertiser newAdvertiser = new Ads.Base.Advertiser
                    {
                        advertiser = selectedAdvertiser,
                        ID = newAdvertiserID,
                        IsAndroid = newAdvertiserIsAndroid,
                        IsIOS = newAdvertiserIsIOS,
                        Ads = new List<AD>()
                    };
                    AdSetting.CurrentPipline.Advertisers.Add(newAdvertiser);
                    EditorUtility.SetDirty(AdSetting);
                    selectedAdvertiser.AddToRegistry();
                }
                else
                    AdvertiserEditorWindow.ShowPanel<InstallPackage>();
            }
            else
                EditorUtility.DisplayDialog("Warning", $"Advertiser [{selectedAdvertiser}] already exist in active list.", "OK");
        }

        private void FilteredAdvertiserDropdown()
        {
            List<SupportedAdvertisers> availableAdvertisers = Enum.GetValues(typeof(SupportedAdvertisers))
                .Cast<SupportedAdvertisers>()
                .Where(advertiser => !AdSetting.CurrentPipline.Advertisers.Any(a => a.advertiser == advertiser))
                .ToList();

            int selectedIndex = availableAdvertisers.IndexOf(selectedAdvertiser);
            selectedIndex = EditorGUILayout.Popup(new GUIContent("Advertiser:", "Select an advertiser to add"), selectedIndex, availableAdvertisers.Select(a => a.ToString()).ToArray());

            if (selectedIndex >= 0 && selectedIndex < availableAdvertisers.Count)
            {
                selectedAdvertiser = availableAdvertisers[selectedIndex];
            }
        }
    }
}
#endif